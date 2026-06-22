using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class ClientManager : SingletonNetwork<ClientManager>
{

    private Dictionary<string, ClientData> connectedClientsMap = new Dictionary<string, ClientData>();
    private Dictionary<ulong, string> connects = new();
    public event Action OnClientListChanged;
    public event Action<ClientData> OnClientDataConnected;
    public event Action<ClientData> OnClientDataDisconnected;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        OnServerStarted();
    }
    private void OnServerStarted()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }
    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ (Memory Leak)
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
    public void OnClientConnected(string playerId, ulong clientId)
    {
        Debug.Log($"[ClientManager] Client {clientId} đã kết nối thành công.");
        if (!IsServer) return;
        if (connectedClientsMap.ContainsKey(playerId)) return;

        ClientData newData = new ClientData
        {
            playerId = playerId,
            clientId = clientId,
            playerObject = NetworkManager.ConnectedClients[clientId].PlayerObject
        };
        if (connectedClientsMap.ContainsKey(playerId))
        {
            connectedClientsMap.Remove(playerId);
        }
        if (connects.ContainsKey(clientId))
        {
            connects.Remove(clientId);
        }
        connectedClientsMap.Add(playerId, newData);
        connects.Add(clientId, playerId);
        OnClientDataConnected?.Invoke(newData);
        OnClientListChanged?.Invoke();
    }

    public void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[ClientManager] Client {clientId} đã ngắt kết nối.");
        if (!IsServer) return;
        if (!connects.TryGetValue(clientId, out string playerId)) return;
        if (!connectedClientsMap.ContainsKey(playerId)) return;

        connectedClientsMap.Remove(playerId);
        connects.Remove(clientId);
    }

    #region PUBLIC API

    public ClientData GetClientData(string PlayerId)
    {
        if (connectedClientsMap.TryGetValue(PlayerId, out var data))
        {
            return data;
        }
        return null;
    }
    public bool ClientOnline(string playerId) => connectedClientsMap.ContainsKey(playerId);

    public Dictionary<string, ClientData> GetAllClients()
    {
        return connectedClientsMap;
    }
    public NetworkObject GetNetworkObject(string playerId)
    {
        if (connectedClientsMap.TryGetValue(playerId, out var data))
        {
            return data.playerObject;
        }
        return null;
    }
    #endregion
}