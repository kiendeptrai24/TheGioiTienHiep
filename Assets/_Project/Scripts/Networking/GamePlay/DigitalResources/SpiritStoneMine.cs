using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Unity.Netcode;
using UnityEngine;
[Serializable]
public class MineOwnershipSegment
{
    public string OwnerId;
    public float StartTime;
    public float EndTime;
}
public class SpiritStoneMine : TGTHNetworkBehaviour
{
    private NetworkVariable<float> CurrentMiningProgress = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private NetworkVariable<int> CurrentAmount = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [Header("Owner PlayerId")]
    [SerializeField] private NetworkObject _owner;
    [SerializeField] private ResourceStorage _ownerStorage;
    [SerializeField] private string playerId;

    [Space]

    [Header("Mine Config")]
    [SerializeField] ItemPreset mine;
    [SerializeField] private ItemResourseData miningData;

    public List<MineOwnershipSegment> history;

    private double _lastProduceTime;
    private double _lastSecondTime;
    private double now;

    // ===== OFFLINE MINING =====
    private bool _lastTimeOffline = false;
    [SerializeField] private MineOwnershipSegment oldSegment;

    [SerializeField] private MineOwnershipSegment newSegment;
    private PlayerBattleRoster battleRoster;

    public ItemData GetItemResourseData()
    {
        if (miningData != null)
        {
            miningData.currentMiningProgress = CurrentMiningProgress.Value;
            miningData.currentAmount = CurrentAmount.Value;
        }

        return miningData;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        miningData = mine.GetItemData() as ItemResourseData;
        battleRoster = GetComponent<PlayerBattleRoster>();
    }
    public void ResetResource()
    {
        miningData = mine.GetItemData() as ItemResourseData;
        if (!IsServer)
            return;

        if (_owner != null)
            UnLink(_owner.NetworkObjectId);


        _owner = null;
        _ownerStorage = null;
        playerId = "";

        _lastTimeOffline = true;
    }
    public void SetOwner(ulong netId, Action success = null, Action<string> fail = null)
    {
        if (!IsServer) return;
        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];
        if (owner == _owner)
        {
            fail?.Invoke("Không tìm thấy tài khoản");
            return;
        }

        _ownerStorage = owner.GetComponent<ResourceStorage>();
        var playerProfile = owner.GetComponent<PlayerProfile>();

        if (playerProfile == null)
        {
            fail?.Invoke("Không tìm thấy tài khoản");
            return;
        }

        if (PlayerIsOwner(playerId))
        {
            if (PlayerIsOnline())
            {
                UnLinkRoster(_owner);
                oldSegment = GetSegment(playerId);
                history.Remove(oldSegment);
                oldSegment = null;
            }
            else
            {
                oldSegment = GetSegment(playerId);
                if (oldSegment != null)
                    oldSegment.EndTime = (float)now;
            }
        }
        newSegment = AddHistory(playerProfile.GetPlayerId());

        _owner = owner;
        playerId = playerProfile.GetPlayerId();
        _lastProduceTime = NetworkManager.ServerTime.Time;
        _lastSecondTime = NetworkManager.ServerTime.Time;

        // ===== OFFLINE MINING INIT =====
        _lastTimeOffline = true;

        LinkRoster(_owner);
        var mineLinker = _owner.GetComponent<PlayerMineRelinker>();
        mineLinker?.AddResource(NetworkObjectId);
        success?.Invoke();
    }

    private void LinkRoster(NetworkObject owner)
    {
        var roster = owner.GetComponent<PlayerBattleRoster>();
        if (roster != null)
        {
            roster.OnChampionPlayerChanged += OnChampionPlayerChanged;
        }
    }
    private void UnLinkRoster(NetworkObject owner)
    {
        var roster = owner.GetComponent<PlayerBattleRoster>();
        if (roster != null)
        {
            roster.OnChampionPlayerChanged += OnChampionPlayerChanged;
        }
    }
    private void OnChampionPlayerChanged(List<ItemData> list)
    {
        battleRoster.itemDatas = list;
    }

    private MineOwnershipSegment AddHistory(string playerId, float startTime = -1, float endTime = -1)
    {
        MineOwnershipSegment segment = new MineOwnershipSegment();
        segment.OwnerId = playerId;
        segment.StartTime = startTime;
        segment.EndTime = endTime;
        history.Add(segment);
        return segment;
    }
    private MineOwnershipSegment GetSegment(string playerId)
    {
        foreach (var seg in history)
        {
            if (seg.OwnerId == playerId)
            {
                return seg;
            }
        }
        return null;
    }

    public void UnLink(ulong netId)
    {
        if (!IsServer) return;
        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];

        if (IsObjectOwner(owner) == false) return;

        UnLinkRoster(owner);
        var mineLinker = _owner.GetComponent<PlayerMineRelinker>();
        mineLinker?.AddResource(NetworkObjectId);

        _owner = null;
        _ownerStorage = null;
    }
    public bool PlayerIsOwner(string playerId)
    {
        return this.playerId == playerId;
    }

    private bool PlayerIsOnline()
    {
        return _owner != null && string.IsNullOrEmpty(playerId) == false;
    }
    public bool HasOwner()
    {
        return string.IsNullOrEmpty(playerId) == false;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (miningData.currentMiningProgress >= miningData.maxStorage)
        {
            ResetResource();
            NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject);
            return;
        }

        now = NetworkManager.ServerTime.Time;

        // ===== OFFLINE MINING LOGIC =====
        if (PlayerIsOnline())
        {
            if (_ownerStorage == null)
                return;

            if (now - _lastProduceTime >= miningData.miningTime)
            {
                int ticks = Mathf.FloorToInt((float)(now - _lastProduceTime));
                _lastProduceTime += ticks;
                Produce(ticks);
            }
        }
        else
        {
            if (_lastTimeOffline)
            {
                _lastTimeOffline = false;
                _lastProduceTime = now;
                newSegment.StartTime = (float)now;
                newSegment.EndTime = -1;
            }
        }

        // Update mining progress
        if (now - _lastSecondTime >= 1)
        {
            int ticks = Mathf.FloorToInt((float)(now - _lastSecondTime));
            miningData.currentMiningProgress += ticks;
            CurrentMiningProgress.Value = miningData.currentMiningProgress;
            _lastSecondTime += ticks;
        }
    }

    public bool IsObjectOwner(NetworkObject owner)
    {
        return _owner == owner;
    }

    private void Produce(int times)
    {
        if (!IsServer)
            return;
        var cost = miningData.yieldPerHarvest;
        if (miningData.currentAmount + miningData.yieldPerHarvest > miningData.maxStorage)
        {
            cost = miningData.maxStorage - miningData.currentAmount;
        }
        miningData.currentAmount += cost;
        CurrentAmount.Value = miningData.currentAmount;
        Debug.Log("Produce");
        _ownerStorage.PlusCost((ulong)cost);
    }

    private ulong CalculatePendingOfflineCoins(float startTime, float endTime)
    {
        double offlineDuration = endTime - startTime;
        float yieldPerSecond = miningData.yieldPerHarvest / miningData.miningTime;
        ulong offlineCoinsEarned = (ulong)(yieldPerSecond * offlineDuration);
        return offlineCoinsEarned;
    }
    public ulong GetPendingOfflineCoins(string playerId)
    {
        if (!IsServer)
            return 0;
        ulong cost = 1;
        foreach (var seg in history)
        {
            if (seg.OwnerId == playerId)
            {
                if (seg.EndTime == -1)
                {
                    cost = CalculatePendingOfflineCoins(seg.StartTime, (float)now);
                }
                else
                {
                    cost = CalculatePendingOfflineCoins(seg.StartTime, seg.EndTime);
                }
                break;
            }
        }
        return cost;
    }

    public void AddOfflineCoinsToOwner(ulong netId, string playerId)
    {
        if (!IsServer)
            return;
        MineOwnershipSegment segment = null;
        foreach (var seg in history)
        {
            if (seg.OwnerId == playerId && seg.EndTime != -1)
            {
                var targetStorage = NetworkManager.SpawnManager.SpawnedObjects[netId].GetComponent<ResourceStorage>();

                if (targetStorage == null)
                    return;

                ulong cost = CalculatePendingOfflineCoins(seg.StartTime, seg.EndTime);
                targetStorage.AddOfflineCoins(cost);
                miningData.currentAmount += (int)cost;
                CurrentAmount.Value = miningData.currentAmount;
                segment = seg;
                break;
            }
            else if (seg.OwnerId == playerId && seg.EndTime == -1)
            {

                var targetStorage = NetworkManager.SpawnManager.SpawnedObjects[netId].GetComponent<ResourceStorage>();

                if (targetStorage == null)
                    return;

                ulong cost = CalculatePendingOfflineCoins(seg.StartTime, (float)now);
                targetStorage.AddOfflineCoins(cost);
                SetOwner(targetStorage.NetworkObjectId);
                miningData.currentAmount += (int)cost;
                CurrentAmount.Value = miningData.currentAmount;
                segment = seg;
                break;
            }
        }
        if (segment == null) return;
        history.Remove(segment);
    }
}
