

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleSimulatorRequest : SingletonNetwork<BattleSimulatorRequest>
{

    private void RequestBattleSimulator(ulong playerClientId, ulong monsterNetId)
    {
        if (!IsServer) return;

        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var playerNet)) return;
        if (!NetworkManager.SpawnManager.SpawnedObjects
            .TryGetValue(monsterNetId, out var enemyNO))
            return;

        var enemyObj = enemyNO.gameObject;
        var playerObj = playerNet.PlayerObject;

        if (playerObj == null) return;
        if (enemyObj == null) return;

        var heroRoster = playerObj.GetComponent<PlayerBattleRoster>();
        var enemyRoster = enemyObj.GetComponent<PlayerBattleRoster>();

        List<UnitInput> enemySnaps = new();
        List<UnitInput> heroSnaps = new();
        // HERO uid: 0..H-1
        int heroCount = 0;
        foreach (var heroPrefab in heroRoster.heroPrefabs)
        {
            if (heroPrefab == null) continue;

            // nếu đây là prefab (NetworkObject) -> phải instantiate để lấy StatsData runtime
            var stats = heroPrefab.GetComponent<StatsData>();
            stats.SetupDataPreset(); // hoặc SetupDataPreset/Setup tùy bạn
            var snap = SnapshotMapper.FromStats(stats, heroCount, TeamId.Heroes);
            snap.placement.cell = new Vector2Int(0, heroCount);
            snap.placement.attackRange = (int)snap.snap.attackRange;
            heroSnaps.Add(snap);
            heroCount++;
        }

        int enemyCount = 10;

        // ENEMY uid: heroCount..heroCount+E-1
        foreach (var enemyPrefab in enemyRoster.heroPrefabs)
        {
            if (enemyPrefab == null) continue;

            var stats = enemyPrefab.GetComponent<StatsData>();
            stats.SetupDataPreset();

            var snap = SnapshotMapper.FromStats(stats, enemyCount, TeamId.Enemies);
            snap.placement.cell = new Vector2Int(9, 19 - enemyCount);
            snap.placement.attackRange = (int)snap.snap.attackRange;
            enemySnaps.Add(snap);
            enemyCount++;
        }
        Board board = new Board
        {
            width = 10,
            height = 10,
            moveInterval = 0.25f,
            allowDiagonal = true
        };

        uint seed = (uint)(playerClientId.GetHashCode() ^ monsterNetId.GetHashCode() ^ Environment.TickCount);


        BattleBoardGrid boardGrid = new BattleBoardGrid(board.width, board.height, board.moveInterval, board.allowDiagonal);

        var res = BattleSimulator.Simulate(heroSnaps, enemySnaps, seed, boardGrid, 60f);
        // convert to DTO
        var dto = new BattleEventDTO[res.events.Count];
        for (int i = 0; i < res.events.Count; i++)
        {
            var ev = res.events[i];
            dto[i] = BattleEventMapper.ToDTO(ev);
        }

        SendReplayToClientClientRpc(res.winner.ToString(), res.duration, dto,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { playerClientId } }
            });
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void RequestBattleSimulatorServerRpc(ulong playerClientId, ulong monsterNetId)
    {
        RequestBattleSimulator(playerClientId, monsterNetId);
    }
    [ClientRpc]
    private void SendReplayToClientClientRpc(string winner, float duration, BattleEventDTO[] events, ClientRpcParams rpcParams = default)
    {
        Debug.Log($"Đội chiến thắng là: {winner} với thời gian {duration} giây");
        string text = "";
        // convert to Data
        List<BattleEvent> battleEvents = new List<BattleEvent>();
        foreach (var dto in events)
        {
            battleEvents.Add(BattleEventMapper.FromDTO(dto));
        }

        for (int i = 0; i < battleEvents.Count; i++)
        {
            var ev = battleEvents[i];

            switch (ev)
            {
                case BattleEventMove m:
                    text += $"{m.t:0.00}s MOVE uid: {m.ownerUid} {m.from} -> {m.to}\n\n";
                    break;

                case BattleEventSkill s:
                    text += $"{s.t:0.00}s SKILL uid: {s.attackerUid} -> {s.targetUid} " +
                            $"skillId={s.skillId} dmg={s.damage} crit={s.isCrit} hpAfter={s.targetHpAfter}\n\n";
                    break;
                case BattleEventAttack a:
                    text += $"{a.t:0.00}s ATK uid: {a.attackerUid} -> uid: {a.targetUid} " +
                            $"dmg={a.damage} crit={a.isCrit} hpAfter={a.targetHpAfter}\n\n";
                    break;
                case BattleEventDealth d:
                    text += $"{d.t:0.00}s DEATH uid: {d.attackerUid} killed -> uid: {d.targetUid}\n\n";
                    break;
                default:
                    text += $"{ev.t:0.00}s {ev.type} uid: {ev.ownerUid}\n\n";
                    break;
            }
        }

        Debug.Log(text);
    }

}