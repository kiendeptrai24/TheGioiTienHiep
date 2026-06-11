using Unity.Netcode;
using UnityEngine;
public class NetworkVisibilityChecker : TGTHNetworkBehaviour
{
    [Header("Distance Settings")]
    public float maxDistance = 5;
    private int defauseDistance = 10;
    protected override void Awake() {
        
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        DistanceVisibilityManager.Instance?.Register(this);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        DistanceVisibilityManager.Instance?.Unregister(this);
    }

    // Owner gọi hàm này để thay đổi range
    public void SetVisibilityRange(float newRange)
    {
        if (!IsOwner) return;
        SetVisibilityRangeServerRpc(newRange);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SetVisibilityRangeServerRpc(float newRange)
    {
        maxDistance = Mathf.Max(0f, newRange); // tránh giá trị âm
    }
}