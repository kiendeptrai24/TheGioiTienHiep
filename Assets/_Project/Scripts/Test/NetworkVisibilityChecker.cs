using Unity.Netcode;
using UnityEngine;

public class NetworkVisibilityChecker : TGTHNetworkBehaviour
{
    [Header("Distance Settings")]
    public float VisibilityDistance = 10f;
    [Header("Visibility Check")]
    public float CheckInterval = 0.2f;
    private float lastCheckTime = 0f;
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

    private void OnStatReady(StatsData data)
    {
        float newDistance = Mathf.Max(VisibilityDistance, data.SpiritRange);

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
        VisibilityDistance = Mathf.Max(0f, newRange);
    }
    /// <summary>
    /// This is automatically invoked when spawning the network prefab
    /// relative to each client.
    /// </summary>
    /// <param name="clientId">client identifier to check</param>
    /// <returns>true/false whether it is visible to the client or not</returns>
    private bool CheckVisibility(ulong clientId)
    {
        // If not spawned, then always return false
        if (!IsSpawned)
        {
            return false;
        }

        // Nếu client chưa có PlayerObject (vừa kết nối, chưa spawn) → không visible
        if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out var client) || client.PlayerObject == null)
        {
            return false;
        }

        // We can do a simple distance check between the NetworkObject instance position and the client
        return Vector3.Distance(client.PlayerObject.transform.position, transform.position) <= VisibilityDistance;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // The server handles visibility checks and should subscribe when spawned locally on the server-side.
            NetworkObject.CheckObjectVisibility += CheckVisibility;
            // If we want to continually update, we don't need to check every frame but should check at least once per tick
            NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
        }
        base.OnNetworkSpawn();
    }

    private void OnNetworkTick()
    {
        if (Time.time - lastCheckTime < CheckInterval)
            return;

        lastCheckTime = Time.time;
        // If CheckObjectVisibility is enabled, check the distance to clients
        // once per network tick.
        foreach (var clientId in NetworkManager.ConnectedClientsIds)
        {
            var shouldBeVisibile = CheckVisibility(clientId);
            var isVisibile = NetworkObject.IsNetworkVisibleTo(clientId);
            if (shouldBeVisibile && !isVisibile)
            {
                // Note: This will invoke the CheckVisibility check again
                NetworkObject.NetworkShow(clientId);
            }
            else if (!shouldBeVisibile && isVisibile)
            {
                NetworkObject.NetworkHide(clientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkObject.CheckObjectVisibility -= CheckVisibility;
            NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        }
        base.OnNetworkDespawn();
    }
}
