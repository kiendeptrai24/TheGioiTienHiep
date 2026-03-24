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
    public ItemData GetItemResourseData()
    {
        return miningData;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        miningData = mine.GetItemData() as ItemResourseData;
    }
    public void SetOwner(NetworkObject owner)
    {
        if (!IsServer) return;
        if (owner == _owner) return;
        _owner = owner;
        _ownerStorage = owner.GetComponent<ResourceStorage>();
        _lastProduceTime = NetworkManager.ServerTime.Time;
        _lastSecondTime = NetworkManager.ServerTime.Time;
    }
    public void UnLink(NetworkObject owner)
    {
        if (!IsServer) return;
        if (IsObjectOwner(owner) == false) return;
        _owner = null;
        _ownerStorage = null;
    }
    public bool HasOnwer()
    {
        return _owner != null;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (_ownerStorage == null)
            return;

        now = NetworkManager.ServerTime.Time;
        if (now - _lastProduceTime >= miningData.miningTime)
        {
            int ticks = Mathf.FloorToInt((float)(now - _lastProduceTime));
            _lastProduceTime += ticks;
            Produce(ticks);
        }
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
        miningData.currentAmount += miningData.yieldPerHarvest;
        _ownerStorage.Add(miningData.yieldPerHarvest);
    }
}
