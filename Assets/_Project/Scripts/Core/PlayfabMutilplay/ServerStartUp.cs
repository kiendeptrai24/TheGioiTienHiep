using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ServerStartUp : Singleton<ServerStartUp>
{
    public Configuration configuration;
    private UnityTransport transport;
    protected override void Start()
    {
        if (configuration.IsServerRemoteBuild())
        {
            StartRemoteServer();
        }
    }
    public void StartServer()
    {
        if (configuration.startwithHost) return;
        NetworkManager.Singleton.StartServer();
    }

    private void StartRemoteServer()
    {
        Debug.Log("[ServerStartUp].StartRemoteServer");

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (transport != null)
        {
            ushort portToUse = configuration.port;
            transport.SetConnectionData("0.0.0.0", portToUse, "0.0.0.0");
            Debug.Log($"Server bind UDP port: {portToUse}");
        }
        NetworkManager.Singleton.StartServer();
    }

    private void OnServerStarted()
    {
        Debug.Log("Server Started");
    }

    private void OnClientConnected(ulong clientId)
    {
    }

    private void OnClientDisconnected(ulong clientId)
    {

    }
}
