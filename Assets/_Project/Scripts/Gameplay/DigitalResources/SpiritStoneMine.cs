using Unity.Netcode;
using UnityEngine;

public class SpiritStoneMine : TGTHNetworkBehaviour
{
    [Header("Mine Config")]
    public int stonePerSecond = 1;

    [SerializeField] private ResourceStorage _ownerStorage;

    private double _lastProduceTime;
    private double now;
    public void SetOwner(NetworkObject owner)
    {
        if (!IsServer) return;

        _ownerStorage = owner.GetComponent<ResourceStorage>();
        _lastProduceTime = NetworkManager.ServerTime.Time;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (_ownerStorage == null)
            return;

        now = NetworkManager.ServerTime.Time;

        if (now - _lastProduceTime >= 1.0f)
        {
            int ticks = Mathf.FloorToInt((float)(now - _lastProduceTime));
            _lastProduceTime += ticks;

            Produce(ticks);
        }
    }

    private void Produce(int times)
    {
        _ownerStorage.Add(stonePerSecond * times);
    }
}
