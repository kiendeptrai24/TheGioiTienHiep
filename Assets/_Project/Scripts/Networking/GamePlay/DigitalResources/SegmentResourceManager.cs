

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SegmentResourceManager : SingletonNetwork<SegmentResourceManager>, ISegmentSystem
{
    // mine runtime
    private readonly Dictionary<string, MineRuntimeData> _mineRuntime = new();
    // link between player and mine in runtime
    private readonly Dictionary<string, HashSet<string>> _playerMines = new();
    // persistent sessions
    private readonly Dictionary<string, List<MineOwnershipSession>> _mineSessions = new();
    // ───────── REGISTER ─────────
    public void RegisterMine(string persistentId, ulong networkObjectId, int stonePerSecond)
    {
        if (!IsServer) return;

        _mineRuntime[persistentId] = new MineRuntimeData
        {
            IsAlive = true,
            NetworkObjectId = networkObjectId
        };
        _mineSessions.TryAdd(persistentId, new List<MineOwnershipSession>());
    }

    // ───────── OWNER ─────────

    public void ChangeMineOwner(string persistentId, string newPlayerId, int stonePerSecond, ulong mineNetId)
    {
        if (!IsServer) return;
        if (!_mineSessions.TryGetValue(persistentId, out var sessions))
        {
            _mineSessions[persistentId] = sessions = new List<MineOwnershipSession>();
            _mineRuntime[persistentId] = new MineRuntimeData
            {
                IsAlive = true,
                NetworkObjectId = mineNetId
            };
        }

        long now = TimeUtils.DateTimeOffset();

        ClearOldOwner(persistentId, sessions, now);

        sessions.Add(new MineOwnershipSession
        {
            PlayerId = newPlayerId,
            MineId = persistentId,
            StartTime = now,
            EndTime = 0,
            YieldPerSecond = (ulong)stonePerSecond,
            OfflineTime = 0
        });

        // Thêm vào index của owner mới
        if (!_playerMines.TryGetValue(newPlayerId, out var set))
            _playerMines[newPlayerId] = set = new HashSet<string>();
        set.Add(persistentId);
    }
    // clear old owner
    private void ClearOldOwner(string persistentId, List<MineOwnershipSession> sessions, long now)
    {
        // Xóa mine khỏi index của owner cũ
        if (sessions.FindLast(s => s.EndTime == 0) is { } prev)
        {
            // Nếu owner cũ đang offline thì giữ lại index để trả reward pending khi họ online lại.
            if (prev.OfflineTime == 0)
                RemoveFromPlayerIndex(prev.PlayerId, persistentId);
        }

        CloseActiveSession(sessions, now);

        // Xóa session đã đóng lâu (giữ lại tối đa N session gần nhất nếu cần log)
        TrimOldSessions(sessions);
    }
    // ───────── MINE DEAD ─────────

    public void OnMineDead(string persistentId)
    {
        if (!IsServer) return;

        if (_mineSessions.TryGetValue(persistentId, out var sessions))
        {
            long now = TimeUtils.DateTimeOffset();
            CloseActiveSession(sessions, now);

            // Xóa mine khỏi index của owner hiện tại
            var active = sessions.FindLast(s => s.PlayerId != null);
            if (active != null)
                RemoveFromPlayerIndex(active.PlayerId, persistentId);

            TrimOldSessions(sessions);
        }

        if (_mineRuntime.TryGetValue(persistentId, out var data))
        {
            // Là class nên update tại chỗ, không cần replace
            data.IsAlive = false;
            data.NetworkObjectId = 0;
        }
    }

    // ───────── HELPERS ─────────

    public void RemoveFromPlayerIndex(string playerId, string mineId)
    {
        if (playerId == null) return;
        if (_playerMines.TryGetValue(playerId, out var set))
            set.Remove(mineId);
    }

    // Chỉ giữ session đang active — session đã đóng không cần thiết nữa
    // (nếu cần audit log thì giữ lại N session gần nhất)
    private static void TrimOldSessions(List<MineOwnershipSession> sessions,
                                        int keepClosed = 0)
    {
        sessions.RemoveAll(s => s.EndTime != 0
                             && s.OfflineTime == 0
                             && keepClosed == 0);
    }

    private static void CloseActiveSession(List<MineOwnershipSession> sessions, long now)
    {
        foreach (var s in sessions)
            if (s.EndTime == 0) { s.EndTime = now; break; }
    }

    private void RestoreOwnedMines(string playerId, ulong playerNetId)
    {
        if (!_playerMines.TryGetValue(playerId, out var mineIds)) return;

        foreach (var mineId in new List<string>(mineIds))
        {
            if (!_mineRuntime.TryGetValue(mineId, out var runtime)) continue;
            if (!runtime.IsAlive) continue;

            if (!_mineSessions.TryGetValue(mineId, out var sessions)) continue;
            var active = sessions.FindLast(s => s.EndTime == 0);
            if (active?.PlayerId != playerId) continue;

            RestoreMineForPlayer(playerNetId, runtime.NetworkObjectId);
        }
    }

    private void RestoreMineForPlayer(ulong playerNetId, ulong mineNetId)
    {
        // 1. Chỉ Server mới có quyền cấu hình lại thế giới và đổi chủ vật thể
        if (!IsServer) return;
        if (!NetworkManager.SpawnManager.SpawnedObjects
            .TryGetValue(mineNetId, out var mineObject)) return;
        var mine = mineObject.GetComponent<SpiritStoneMine>();
        mine?.SetOwner(playerNetId, true);
    }

    private bool TryGetResourceStorage(ulong networkObjectId, out ResourceStorage storage)
    {
        storage = null;
        if (!NetworkManager.SpawnManager.SpawnedObjects
            .TryGetValue(networkObjectId, out var netObj)) return false;

        storage = netObj.GetComponent<ResourceStorage>();
        return storage != null;
    }
    public bool TryGetNetworkObjectId(ulong clientId, out ulong networkObjectId)
    {
        networkObjectId = 0;

        if (NetworkManager.ConnectedClients.TryGetValue(clientId, out var networkClient) &&
            networkClient.PlayerObject != null)
        {
            networkObjectId = networkClient.PlayerObject.NetworkObjectId;
            return true;
        }

        return false;
    }

    public void ConnectSegment(ClientData data)
    {
        if (!IsServer) return;
        string playerId = data.playerId;
        ulong localClientId = data.clientId;

        if (!TryGetNetworkObjectId(localClientId, out var playerNetId))
            return;
        if (!TryGetResourceStorage(playerNetId, out var resourceStorage)) return;
        if (!_playerMines.TryGetValue(playerId, out var mineIds)) return;

        ulong totalReward = 0;
        long now = TimeUtils.DateTimeOffset();
        var mineIdsToRemove = new List<string>();

        foreach (var mineId in new List<string>(mineIds))
        {
            if (!_mineSessions.TryGetValue(mineId, out var sessions)) continue;

            foreach (var session in sessions)
            {
                if (session.PlayerId != playerId) continue;
                if (session.OfflineTime == 0) continue;

                long sessionEnd = session.EndTime == 0 ? now : session.EndTime;
                if (sessionEnd <= session.OfflineTime) continue;

                long rewardStart = Math.Max(session.StartTime, session.OfflineTime);
                long duration = sessionEnd - rewardStart;
                if (duration <= 0) continue;

                totalReward += (ulong)(duration * (long)session.YieldPerSecond);
                session.OfflineTime = 0;

                Debug.Log($"Mine: {mineId} | Duration: {duration}s | Reward: {totalReward}");
            }

            bool hasActiveOwnedSession = sessions.Exists(s => s.PlayerId == playerId && s.EndTime == 0);
            bool hasPendingOfflineReward = sessions.Exists(s => s.PlayerId == playerId && s.OfflineTime != 0);
            if (!hasActiveOwnedSession && !hasPendingOfflineReward)
                mineIdsToRemove.Add(mineId);
        }

        if (totalReward > 0)
            resourceStorage.PlusCost(totalReward);

        foreach (var mineId in mineIdsToRemove)
            RemoveFromPlayerIndex(playerId, mineId);

        RestoreOwnedMines(playerId, playerNetId);
    }

    public void DisconnectSegment(ClientData data)
    {
        if (!IsServer) return;
        string playerId = data.playerId;
        if (!_playerMines.TryGetValue(playerId, out var mineIds)) return;

        long now = TimeUtils.DateTimeOffset();

        foreach (var mineId in new List<string>(mineIds))
        {
            if (!_mineSessions.TryGetValue(mineId, out var sessions)) continue;

            foreach (var session in sessions)
            {
                if (session.PlayerId == playerId && session.EndTime == 0)
                {
                    session.OfflineTime = now;
                    break; // mỗi mine chỉ có 1 active session
                }
            }
        }
    }
}