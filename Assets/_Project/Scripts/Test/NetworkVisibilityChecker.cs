using System;
using Unity.Netcode;
using UnityEngine;
public class NetworkVisibilityChecker : TGTHNetworkBehaviour
{
    [Header("Distance Settings")]
    public float distance = 10;
    private StatsData statsData;
    protected override void Awake()
    {
        statsData = GetComponent<StatsData>();
        statsData.OnStatReady += OnStatReady;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        DistanceVisibilityManager.Instance?.Register(this);
    }

    private void OnStatReady(StatsData data)
    {
        float distance = Mathf.Max(this.distance, data.SpiritRange);
        SetVisibilityRangeServerRpc(distance);
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
        distance = Mathf.Max(0f, newRange);
    }
}