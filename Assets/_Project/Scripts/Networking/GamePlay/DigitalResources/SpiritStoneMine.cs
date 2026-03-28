using Unity.Netcode;
using UnityEngine;

public class SpiritStoneMine : TGTHNetworkBehaviour
{
    [Header("Owner PlayerId")]
    [SerializeField] private NetworkObject _owner;
    [SerializeField] private ResourceStorage _ownerStorage;
    [SerializeField] private string playerId;

    [Space]

    [Header("Mine Config")]
    [SerializeField] ItemPreset mine;
    [SerializeField] private ItemResourseData miningData;

    private double _lastProduceTime;
    private double _lastSecondTime;
    private double now;

    // ===== OFFLINE MINING =====
    private bool _lastTimeOffline = false;


    public ItemData GetItemResourseData()
    {
        return miningData;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        miningData = mine.GetItemData() as ItemResourseData;
    }
    public void ResetResource()
    {
        if (!IsServer) return;
        if (_owner != null)
            UnLink(_owner.NetworkObjectId);


        _owner = null;
        _ownerStorage = null;
        playerId = "";

        _lastTimeOffline = true;
        miningData = mine.GetItemData() as ItemResourseData;
    }
    public void SetOwner(ulong netId)
    {
        if (!IsServer) return;
        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];
        if (owner == _owner) return;

        _owner = owner;
        _ownerStorage = owner.GetComponent<ResourceStorage>();
        var playerProfile = _owner.GetComponent<PlayerProfile>();

        if (playerProfile == null) return;
        playerId = playerProfile.GetPlayerId();
        _lastProduceTime = NetworkManager.ServerTime.Time;
        _lastSecondTime = NetworkManager.ServerTime.Time;

        // ===== OFFLINE MINING INIT =====
        _lastTimeOffline = true;

        miningData.lastOwnerClaimTime = NetworkManager.ServerTime.Time;

        var mineLinker = _owner.GetComponent<PlayerMineRelinker>();
        mineLinker?.AddResource(NetworkObjectId);
    }
    public void UnLink(ulong netId)
    {
        if (!IsServer) return;
        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];

        if (IsObjectOwner(owner) == false) return;

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
        return _owner && string.IsNullOrEmpty(playerId) == false;
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
                miningData.lastOwnerClaimTime = now;
            }
        }

        // Update mining progress
        if (now - _lastSecondTime >= 1)
        {
            int ticks = Mathf.FloorToInt((float)(now - _lastSecondTime));
            miningData.currentMiningProgress += ticks;
            _lastSecondTime += ticks;
        }
    }

    public bool IsObjectOwner(NetworkObject owner)
    {
        return _owner == owner;
    }

    private void Produce(int times)
    {
        var cost = miningData.yieldPerHarvest;
        if (miningData.currentAmount + miningData.yieldPerHarvest > miningData.maxStorage)
        {
            cost = miningData.maxStorage - miningData.currentAmount;
        }
        miningData.currentAmount += cost;
        Debug.Log("Produce");
        _ownerStorage.PlusCost((ulong)cost);
    }

    private void CalculatePendingOfflineCoins()
    {
        double offlineDuration = now - miningData.lastOwnerClaimTime;
        float yieldPerSecond = miningData.yieldPerHarvest / miningData.miningTime;
        ulong offlineCoinsEarned = (ulong)(yieldPerSecond * offlineDuration);

        miningData.accumulatedOfflineCoins = offlineCoinsEarned;

        Debug.Log($"[SpiritStoneMine] CalculatePending: duration={offlineDuration}s, yield/sec={yieldPerSecond}, total={miningData.accumulatedOfflineCoins}");
    }

    public ulong GetPendingOfflineCoins()
    {
        if (!IsServer)
            return 0;
        CalculatePendingOfflineCoins();
        return miningData.accumulatedOfflineCoins;
    }

    public void AddOfflineCoinsToOwner(ulong netId, string playerId)
    {
        if (!IsServer)
            return;
        if (PlayerIsOwner(playerId) == false)
            return;

        var targetStorage = NetworkManager.SpawnManager.SpawnedObjects[netId].GetComponent<ResourceStorage>();

        if (targetStorage == null)
            return;

        CalculatePendingOfflineCoins();

        if (miningData.accumulatedOfflineCoins > 0)
        {
            targetStorage.AddOfflineCoins(miningData.accumulatedOfflineCoins);
        }
        SetOwner(targetStorage.NetworkObjectId);
    }
}
