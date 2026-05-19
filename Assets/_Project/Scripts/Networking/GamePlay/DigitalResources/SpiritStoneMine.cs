using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpiritStoneMine : TGTHNetworkBehaviour
{
    #region Networkvariable
    public NetworkVariable<float> CurrentMiningProgress = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> CurrentAmount = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<FixedString64Bytes> PlayerId = new(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<ulong> OwnerNetworkId = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    #endregion

    public MineNetworkState networkState;
    public IOwnerShip ownership;
    public MineProductionSystem production;
    public IRosterLinker rosterLinker;

    [SerializeField]
    private SpiritStoneMineData miningData;
    private ResourceStorage ownerStorage;
    private ResourceNode itemMapWorld;
    private SegmentResourceManager segmentMineManager;
    protected override void Awake()
    {
        base.Awake();
        CurrentMiningProgress.OnValueChanged += (oldValue, newValue) =>
        {
            if (miningData == null) return;
            miningData.currentMiningProgress = CurrentMiningProgress.Value;
        };
        CurrentAmount.OnValueChanged += (oldValue, newValue) =>
        {
            if (miningData == null) return;
            miningData.currentAmount = CurrentAmount.Value;
        };
        PlayerId.OnValueChanged += (oldValue, newValue) =>
        {
            if (miningData == null) return;
            networkState.playerId = PlayerId.Value.ToString();
        };
    }

    public override void OnNetworkSpawn()
    {
        segmentMineManager = SegmentResourceManager.Instance;
        itemMapWorld = GetComponent<ResourceNode>();
        base.OnNetworkSpawn();
        miningData = itemMapWorld.GetData() as SpiritStoneMineData;
        networkState = new MineNetworkState();
        int yieldPerSecond = Mathf.RoundToInt(miningData.yieldPerHarvest / miningData.miningTime);

        segmentMineManager.RegisterMine(
            miningData.resourceId,
            NetworkObjectId,
            yieldPerSecond);

        ownership =
            new MineOwnershipSystem(
                networkState);

        production =
            new MineProductionSystem(
                miningData,
                networkState);

        rosterLinker =
            new MineRosterLinker(GetComponent<PlayerBattleRoster>());


    }
    private void Update()
    {
        if (!IsServer)
            return;

        if (!ownership.HasOwner())
            return;

        if (ownership.IsOnline())
        {
            production.Tick(
                TimeUtils.GetServerTime(),
                ownerStorage
            );
        }
    }
    public void SetOwner(ulong netId)
    {
        if (!IsServer)
            return;

        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];
        float now = (float)NetworkManager.ServerTime.Time;
        FixedString64Bytes playerId = owner.GetComponent<PlayerProfile>().GetPlayerId();

        ownership.SetOwner(owner, playerId.ToString(), now);
        int yieldPerSecond = Mathf.RoundToInt(miningData.yieldPerHarvest / miningData.miningTime);
        segmentMineManager.ChangeMineOwner(miningData.resourceId, playerId.ToString(), yieldPerSecond);
    }
    public void UnSetOwner(ulong netId)
    {
        if (!IsServer) return;

        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];
        PlayerProfile playerProfile = owner.GetComponent<PlayerProfile>();

        if (playerProfile == null) return;

        var playerId = playerProfile.GetPlayerId();
        if (ownership.IsOwner(playerId.ToString()) == false) return;

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