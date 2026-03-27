using Unity.Netcode;
using UnityEngine;

public class SpiritStoneMine : TGTHNetworkBehaviour
{
    [Header("Mine Config")]
    [SerializeField] ItemPreset mine;
    [SerializeField] private ItemResourseData miningData;

    [SerializeField] private ResourceStorage _ownerStorage;
    private NetworkObject _owner;
    private double _lastProduceTime;
    private double _lastSecondTime;
    private double now;

    // ===== OFFLINE MINING =====
    private bool _ownerIsOffline = false;
    private ulong _pendingOfflineCoins = 0;
    private double _offlineMiningStartTime = 0;
    public ItemData GetItemResourseData()
    {
        return miningData;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        miningData = mine.GetItemData() as ItemResourseData;
    }
    public void SetOwner(ulong netId)
    {
        if (!IsServer) return;
        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];
        if (owner == _owner) return;
        _owner = owner;
        _ownerStorage = owner.GetComponent<ResourceStorage>();
        _lastProduceTime = NetworkManager.ServerTime.Time;
        _lastSecondTime = NetworkManager.ServerTime.Time;

        // ===== OFFLINE MINING INIT =====
        _ownerIsOffline = false;
        _pendingOfflineCoins = 0;
        _offlineMiningStartTime = NetworkManager.ServerTime.Time;
        miningData.lastOwnerClaimTime = NetworkManager.ServerTime.Time;
        Debug.Log($"[SpiritStoneMine] SetOwner: {netId}, offlineStart={_offlineMiningStartTime}");
    }
    public void UnLink(ulong netId)
    {
        if (!IsServer) return;
        var owner = NetworkManager.SpawnManager.SpawnedObjects[netId];
        if (IsObjectOwner(owner) == false) return;

        // ===== CALCULATE PENDING COINS BEFORE UNLINK =====
        CalculatePendingOfflineCoins();
        if (_pendingOfflineCoins > 0 && _ownerStorage != null)
        {
            _ownerStorage.AddOfflineCoins(_pendingOfflineCoins);
            miningData.accumulatedOfflineCoins += _pendingOfflineCoins;
            Debug.Log($"[SpiritStoneMine] UnLink: Sent {_pendingOfflineCoins} pending coins, total accumulated: {miningData.accumulatedOfflineCoins}");
        }

        _owner = null;
        _ownerStorage = null;
        _ownerIsOffline = false;
        _pendingOfflineCoins = 0;
    }
    public bool HasOnwer()
    {
        return _owner != null;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        now = NetworkManager.ServerTime.Time;

        // ===== OFFLINE MINING LOGIC =====
        if (_owner != null)
        {
            // Owner is online - normal mining
            if (_ownerIsOffline)
            {
                _ownerIsOffline = false;
                Debug.Log("[SpiritStoneMine] Owner is back online!");
            }

            if (_ownerStorage == null)
                return;

            if (now - _lastProduceTime >= miningData.miningTime)
            {
                int ticks = Mathf.FloorToInt((float)(now - _lastProduceTime));
                _lastProduceTime += ticks;
                Produce(ticks);
            }
        }
        else if (!_ownerIsOffline && _offlineMiningStartTime > 0)
        {
            // Owner went offline - start accumulating
            _ownerIsOffline = true;
            _lastProduceTime = now;
            Debug.Log("[SpiritStoneMine] Owner is offline, starting offline mining accumulation");
        }

        // Update mining progress (both online and offline)
        if (now - _lastSecondTime >= 1)
        {
            int ticks = Mathf.FloorToInt((float)(now - _lastSecondTime));
            miningData.currentMiningProgress += ticks;
            _lastSecondTime += ticks;

            // Accumulate offline coins every second
            if (_ownerIsOffline && _offlineMiningStartTime > 0)
            {
                CalculateOfflineMiningPerSecond();
            }
        }
    }
    public bool IsObjectOwner(NetworkObject owner)
    {
        return _owner == owner;
    }

    private void Produce(int times)
    {
        miningData.currentAmount += miningData.yieldPerHarvest;
        Debug.Log("Produce");
        _ownerStorage.PlusCost((ulong)miningData.yieldPerHarvest);
    }

    // ===== OFFLINE MINING HELPERS =====
    private void CalculateOfflineMiningPerSecond()
    {
        // Calculate mining per second: yieldPerHarvest / miningTime
        float yieldPerSecond = miningData.yieldPerHarvest / miningData.miningTime;
        ulong coinsPerSecond = (ulong)yieldPerSecond;

        if (coinsPerSecond > 0)
        {
            _pendingOfflineCoins += coinsPerSecond;
        }
    }

    private void CalculatePendingOfflineCoins()
    {
        if (_offlineMiningStartTime <= 0)
            return;

        double offlineDuration = now - miningData.lastOwnerClaimTime;
        float yieldPerSecond = miningData.yieldPerHarvest / miningData.miningTime;
        ulong offlineCoinsEarned = (ulong)(yieldPerSecond * offlineDuration);

        _pendingOfflineCoins = offlineCoinsEarned;
        Debug.Log($"[SpiritStoneMine] CalculatePending: duration={offlineDuration}s, yield/sec={yieldPerSecond}, total={_pendingOfflineCoins}");
    }

    public ulong GetPendingOfflineCoins()
    {
        if (!IsServer)
            return 0;
        CalculatePendingOfflineCoins();
        return _pendingOfflineCoins;
    }

    public void AddOfflineCoinsToOwner(ResourceStorage targetStorage)
    {
        if (!IsServer)
            return;

        CalculatePendingOfflineCoins();
        if (_pendingOfflineCoins > 0 && targetStorage != null)
        {
            targetStorage.AddOfflineCoins(_pendingOfflineCoins);
            miningData.accumulatedOfflineCoins += _pendingOfflineCoins;
            miningData.lastOwnerClaimTime = NetworkManager.ServerTime.Time;
            Debug.Log($"[SpiritStoneMine] AddOfflineToOwner: Added {_pendingOfflineCoins} coins");
        }
        _pendingOfflineCoins = 0;
    }
}
