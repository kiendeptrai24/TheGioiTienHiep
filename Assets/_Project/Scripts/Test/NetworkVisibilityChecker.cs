using Unity.Netcode;
using UnityEngine;

public class NetworkVisibilityChecker : TGTHNetworkBehaviour
{
    [Header("Distance Settings")]
    public float distance = 10f;

    private StatsData statsData;

    protected override void Awake()
    {
        base.Awake();
        statsData = GetComponent<StatsData>();
        if (statsData != null)
        {
            statsData.OnStatReady += OnStatReady;
        }
    }

    protected void OnDestroy()
    {
        if (statsData != null)
        {
            statsData.OnStatReady -= OnStatReady;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;
        DistanceVisibilityManager.Instance?.Register(this);
        DistanceVisibilityManager.Instance?.RefreshVisibilityForAllClients(this);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            DistanceVisibilityManager.Instance?.Unregister(this);
        }

        base.OnNetworkDespawn();
    }

    private void OnStatReady(StatsData data)
    {
        float newDistance = Mathf.Max(distance, data.SpiritRange);

        if (IsServer)
        {
            ApplyVisibilityRange(newDistance);
            return;
        }

        if (IsOwner)
        {
            SetVisibilityRangeServerRpc(newDistance);
        }
    }

    public void SetVisibilityRange(float newRange)
    {
        if (!IsOwner) return;
        SetVisibilityRangeServerRpc(newRange);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SetVisibilityRangeServerRpc(float newRange)
    {
        ApplyVisibilityRange(newRange);
    }

    private void ApplyVisibilityRange(float newRange)
    {
        distance = Mathf.Max(0f, newRange);

        if (IsServer)
        {
            DistanceVisibilityManager.Instance?.RefreshVisibilityForAllClients(this);
        }
    }
}
