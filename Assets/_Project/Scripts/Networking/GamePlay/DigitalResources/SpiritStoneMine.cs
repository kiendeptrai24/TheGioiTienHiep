using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

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
    [SerializeField]
    public NetworkVariable<FixedString64Bytes> playerId = new(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [Header("Owner PlayerId")]
    [SerializeField] private NetworkObject _owner;
    [SerializeField] private ResourceStorage _ownerStorage;

    [Space]

    [Header("Mine Config")]
    [SerializeField] private SpiritStoneMineData miningData;

    public List<MineOwnershipSegment> history;

    private double _lastProduceTime;
    private double _lastSecondTime;
    private double now;

    // ===== OFFLINE MINING =====
    private bool _lastTimeOffline = false;
    [SerializeField] private MineOwnershipSegment oldSegment;

    [SerializeField] private MineOwnershipSegment newSegment;
    private PlayerBattleRoster battleRoster;
    [SerializeField] private ItemMapWorld itemMapWorld;
    public SpiritStoneMineData GetItemResourseData()
    {
        if (miningData != null)
        {
            miningData.currentMiningProgress = CurrentMiningProgress.Value;
            miningData.currentAmount = CurrentAmount.Value;
        }

        return miningData;
    }
    protected override void Awake()
    {
        base.Awake();
        itemMapWorld = GetComponent<ItemMapWorld>();
        battleRoster = GetComponent<PlayerBattleRoster>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            SpawnMine.Instance.AddNetObject(NetworkObject);
        }
        ResetResource();
    }

    public void ResetResource()
    {
        if (itemMapWorld == null) return;
        itemMapWorld.ResetItemData();
        miningData = itemMapWorld.GetItemData() as SpiritStoneMineData;
        if (!IsServer)
            return;

        if (_owner != null)
            UnLink(_owner.NetworkObjectId);


        _owner = null;
        _ownerStorage = null;
        playerId.Value = "";

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

        if (PlayerIsOwner(playerId.Value))
        {
            if (PlayerIsOnline())
            {
                UnLinkRoster(_owner);
                oldSegment = GetSegment(playerId.Value);
                history.Remove(oldSegment);
                oldSegment = null;
            }
            else
            {
                oldSegment = GetSegment(playerId.Value);
                if (oldSegment != null)
                    oldSegment.EndTime = (float)now;
            }
        }
        newSegment = AddHistory(playerProfile.GetPlayerId().ToString());

        _owner = owner;
        playerId.Value = playerProfile.GetPlayerId();
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

    private MineOwnershipSegment AddHistory(FixedString64Bytes playerId, float startTime = -1, float endTime = -1)
    {
        MineOwnershipSegment segment = new MineOwnershipSegment();
        segment.OwnerId = playerId;
        segment.StartTime = startTime;
        segment.EndTime = endTime;
        history.Add(segment);
        return segment;
    }
    private MineOwnershipSegment GetSegment(FixedString64Bytes playerId)
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
        PlayerProfile playerProfile = owner.GetComponent<PlayerProfile>();

        if (playerProfile == null) return;

        var playerId = playerProfile.GetPlayerId();

        if (PlayerIsOwner(playerId) == false) return;

        UnLinkRoster(owner);
        var mineLinker = _owner.GetComponent<PlayerMineRelinker>();
        mineLinker?.AddResource(NetworkObjectId);

        _owner = null;
        _ownerStorage = null;
        this.playerId.Value = "";
    }
    public bool PlayerIsOwner(FixedString64Bytes playerId)
    {
        return this.playerId.Value == playerId;
    }

    private bool PlayerIsOnline()
    {
        return _owner != null && playerId.Value.IsEmpty == false;
    }
    public bool HasOwner()
    {
        return playerId.Value.IsEmpty == false;
    }
    private void Update()
    {
        if (!IsServer)
            return;
        if (HasOwner() == false) return;

        if (miningData == null)
        {
            miningData = itemMapWorld.GetItemData() as SpiritStoneMineData;
            return;
        }
        if (miningData.currentMiningProgress >= miningData.maxStorage)
        {
            ResetResource();
            SpawnMine.Instance.RemoveNetObject(NetworkObject);
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
        FixedString64Bytes ownerId = new FixedString64Bytes(playerId);
        foreach (var seg in history)
        {
            if (seg.OwnerId == ownerId)
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
        FixedString64Bytes ownerId = new FixedString64Bytes(playerId);
        foreach (var seg in history)
        {
            if (seg.OwnerId == ownerId && seg.EndTime != -1)
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
            else if (seg.OwnerId == ownerId && seg.EndTime == -1)
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
