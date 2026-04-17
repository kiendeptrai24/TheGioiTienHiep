using System;
using UnityEngine;
using Unity.Netcode;

public class ServerNotification : NetworkBehaviour
{
    public static ServerNotification Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer && !NetworkObject.IsSpawned)
        {
            NetworkObject.Spawn();
        }
    }

    [ClientRpc]
    public void ShutdownClientRpc()
    {
        Debug.Log("Client received shutdown signal from server");
    }

    [ClientRpc]
    public void MaintenanceClientRpc(long scheduledUnixTime)
    {
        DateTime scheduled =
            DateTimeOffset.FromUnixTimeSeconds(scheduledUnixTime).UtcDateTime;
        Debug.Log($"Client received maintenance time: {scheduled}");
    }
}
