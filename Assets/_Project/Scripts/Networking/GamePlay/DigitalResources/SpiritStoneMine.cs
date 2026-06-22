using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpiritStoneMine : TGTHNetworkBehaviour
{
    #region Networkvariable
    [SerializeField]
    public NetworkVariable<float> CurrentMiningProgress = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [SerializeField]
    public NetworkVariable<int> CurrentAmount = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [SerializeField]
    private NetworkVariable<FixedString64Bytes> PlayerId = new(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    [SerializeField]
    private NetworkVariable<ulong> OwnerNetworkId = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    #endregion

    [SerializeField] private MineNetworkState networkState;
    public IOwnerShip ownership;
    public MineProductionSystem production;
    public IRosterLinker rosterLinker;

    [SerializeField]
    private SpiritStoneMineData miningData;
    private ResourceStorage ownerStorage;
    private ResourceNode itemMapWorld;
    private SegmentResourceManager segmentMineManager;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SpawnMine.Instance.AddNetObject(NetworkObject);
        segmentMineManager = SegmentResourceManager.Instance;
        itemMapWorld = GetComponent<ResourceNode>();
        AddEvent();
        itemMapWorld.OnDataReady += (itemData) =>
        {
            SetupMine();
        };
        if (itemMapWorld.IsDataReady())
        {
            SetupMine();
        }
    }
    private void UpdateMineData()
    {
        if (miningData == null) return;
        miningData.currentAmount = CurrentAmount.Value;
        miningData.currentMiningProgress = CurrentMiningProgress.Value;
    }
    public override void OnNetworkPreDespawn()
    {
        base.OnNetworkPreDespawn();
        RemoveEvent();
        itemMapWorld.OnDataReady -= (itemData) =>
        {
            SetupMine();
        };
    }
    private void AddEvent()
    {
        if (IsServer) return;
        CurrentMiningProgress.OnValueChanged += (oldValue, newValue) =>
        {
            if (miningData == null) return;
            miningData.currentMiningProgress = newValue;
        };
        CurrentAmount.OnValueChanged += (oldValue, newValue) =>
        {
            if (miningData == null) return;
            miningData.currentAmount = newValue;
            if (newValue <= 0)
                SpawnMine.Instance.RemoveNetObject(NetworkObject);
        };
        PlayerId.OnValueChanged += (oldValue, newValue) =>
        {
            if (miningData == null) return;
            networkState.playerId = PlayerId.Value.ToString();
        };
    }
    private void RemoveEvent()
    {
        if (IsServer) return;
        CurrentMiningProgress.OnValueChanged -= (oldValue, newValue) =>
        {
            if (miningData == null) return;
            miningData.currentMiningProgress = newValue;
        };
        CurrentAmount.OnValueChanged -= (oldValue, newValue) =>
        {
            if (miningData == null) return;
            miningData.currentAmount = newValue;
        };
        PlayerId.OnValueChanged -= (oldValue, newValue) =>
        {
            if (miningData == null) return;
            networkState.playerId = PlayerId.Value.ToString();
        };
    }

    private void SetupMine()
    {
        miningData = itemMapWorld.GetData() as SpiritStoneMineData;
        networkState = new MineNetworkState();
        int yieldPerSecond = Mathf.RoundToInt(miningData.yieldPerHarvest / miningData.miningTime);

        segmentMineManager.RegisterMine(
            miningData.resourceId,
            NetworkObjectId,
            yieldPerSecond);

        ownership =
            new MineOwnershipSystem(networkState);

        production =
            new MineProductionSystem(miningData, networkState);

        rosterLinker =
            new MineRosterLinker(GetComponent<PlayerBattleRoster>());

        // Apply current replicated values when data becomes ready after spawn.
        UpdateMineData();
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (!ownership.HasOwner())
            return;

        if (ownership.IsOnline())
        {
            var success = production.Tick(
                TimeUtils.GetServerTime(),
                ownerStorage
            );
            if (success)
            {
                CurrentAmount.Value = networkState.currentAmount;
                CurrentMiningProgress.Value = networkState.currentMiningProgress;
                miningData.currentAmount = networkState.currentAmount;
                miningData.currentMiningProgress = networkState.currentMiningProgress;
            }
        }
    }
    public void SetOwner(ulong netId, bool isRestore = false)
    {
        if (!IsServer)
            return;

        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];
        float now = (float)NetworkManager.ServerTime.Time;
        FixedString64Bytes playerId = owner.GetComponent<PlayerProfile>().GetPlayerId();
        int yieldPerSecond = Mathf.RoundToInt(miningData.yieldPerHarvest / miningData.miningTime);

        OwnerNetworkId.Value = netId;
        PlayerId.Value = playerId;
        CurrentAmount.Value = miningData.currentAmount;
        CurrentMiningProgress.Value = miningData.currentMiningProgress;

        ownerStorage = owner.GetComponent<ResourceStorage>();
        ownership.SetOwner(owner, playerId.ToString(), now);

        if (isRestore)
            return;

        production.ResetTime(now);
        segmentMineManager.ChangeMineOwner(miningData.resourceId, playerId.ToString(), yieldPerSecond, NetworkObjectId);
    }
    public void UnSetOwner(ulong netId)
    {
        if (!IsServer) return;

        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];
        PlayerProfile playerProfile = owner.GetComponent<PlayerProfile>();

        if (playerProfile == null) return;

        var playerId = playerProfile.GetPlayerId();
        if (ownership.IsOwner(playerId.ToString()) == false) return;

        PlayerId.Value = "";
        OwnerNetworkId.Value = 0;
        ownerStorage = null;
        segmentMineManager.RemoveFromPlayerIndex(playerId.ToString(), miningData.resourceId);
        ownership.ClearOwner();
    }
    public bool HasOwner() => string.IsNullOrEmpty(PlayerId.Value.ToString()) == false;
    public bool IsOnline() => ownership.IsOnline();
    public bool PlayerIsOwner(FixedString64Bytes id) => PlayerId.Value == id;
    public SpiritStoneMineData GetDataResource() => miningData;
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsServer) return;
        segmentMineManager.OnMineDead(miningData.resourceId);
    }
}