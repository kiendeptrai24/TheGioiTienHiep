using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ClientConnectDebug : MonoBehaviour
{
    public string ip = "20.106.165.51";
    public ushort port = 30000;
    [ContextMenu("Connect")]
    public void Connect()
    {
        StartCoroutine(ConnectRoutine());
    }

    private IEnumerator ConnectRoutine()
    {
        var nm = NetworkManager.Singleton;

        if (nm == null)
        {
            Debug.LogError("NetworkManager.Singleton is NULL");
            yield break;
        }

        var transport = nm.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError("UnityTransport is NULL");
            yield break;
        }

        nm.OnClientConnectedCallback += OnConnected;
        nm.OnClientDisconnectCallback += OnDisconnected;
        nm.OnClientStopped += OnClientStopped;
        nm.OnTransportFailure += OnTransportFailure;

        transport.SetConnectionData(ip, port);

        Debug.Log($"Trying to connect to {ip}:{port}");

        bool result = nm.StartClient();

        Debug.Log($"StartClient result: {result}");
        Debug.Log($"After StartClient | IsClient={nm.IsClient}, IsListening={nm.IsListening}, IsConnectedClient={nm.IsConnectedClient}");

        yield return new WaitForSeconds(10f);

        if (nm.IsClient && !nm.IsConnectedClient)
        {
            Debug.LogError("Client started but did not connect after 10 seconds.");
            Debug.LogError("Likely reason: firewall, UDP blocked, wrong IP/port, or server did not receive the connection.");

            nm.Shutdown();
        }
    }

    private void OnConnected(ulong clientId)
    {
        Debug.Log($"Connected to server. ClientId: {clientId}");
    }

    private void OnDisconnected(ulong clientId)
    {
        Debug.LogWarning($"Disconnected. ClientId: {clientId}");
        Debug.LogWarning($"Disconnect reason: {NetworkManager.Singleton.DisconnectReason}");
    }

    private void OnClientStopped(bool isHost)
    {
        Debug.LogWarning($"Client stopped. isHost: {isHost}");
    }

    private void OnTransportFailure()
    {
        Debug.LogError("Transport failure");
    }
}