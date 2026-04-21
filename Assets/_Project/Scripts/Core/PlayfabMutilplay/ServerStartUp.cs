using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using PlayFab;
using Unity.Netcode.Transports.UTP;
using PlayFab.MultiplayerAgent.Model;

public class ServerStartUp : Singleton<ClientStartUp>
{
    public Configuration configuration;

    private List<ConnectedPlayer> _connectedPlayers;
    private UnityTransport transport;
    protected override void Start()
    {
        if (configuration.buildType == BuildType.REMOTE_SERVER)
        {
            StartRemoteServer();
        }
        else if (configuration.buildType == BuildType.LOCAL_SERVER)
        {
            transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            TryStartServerWithAutoPort();
        }
        else
            Destroy(gameObject, 1);
    }

    private void TryStartServerWithAutoPort()
    {
        if (transport == null)
            return;
        int port = transport.ConnectionData.Port;
        int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            transport.ConnectionData.Port = (ushort)port;

            try
            {
                bool ok = NetworkManager.Singleton.StartServer();

                if (ok)
                {
                    Debug.Log("Server started on port " + port);
                    return;
                }
                else
                {
                    Debug.LogWarning("Failed to start server on port " + port);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Port " + port + " is busy. Error: " + e.Message);
            }

            port++;
        }

        Debug.LogError("Cannot start server! All ports are busy.");
    }

    public void OnStartLocalServerButtonClick()
    {
        if (configuration.buildType == BuildType.LOCAL_SERVER)
        {
            NetworkManager.Singleton.StartServer();
        }
    }

    private void StartRemoteServer()
    {
        Debug.Log("[ServerStartUp].StartRemoteServer");
        _connectedPlayers = new List<ConnectedPlayer>();
        PlayFabMultiplayerAgentAPI.Start();
        PlayFabMultiplayerAgentAPI.IsDebugging = configuration.playFabDebugging;
        PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
        PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
        PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
        PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

        // NGO callbacks
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        StartCoroutine(ReadyForPlayers());
        StartCoroutine(ShutdownServerInXTime());
    }

    IEnumerator ShutdownServerInXTime()
    {
        yield return new WaitForSeconds(300f);
        StartShutdownProcess();
    }

    IEnumerator ReadyForPlayers()
    {
        yield return new WaitForSeconds(.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    }

    private void OnServerActive()
    {
        Debug.Log("Server Started From Agent Activation");

        var utp = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (utp != null)
        {
            ushort portToUse = configuration.port;

            var connectionInfo = PlayFabMultiplayerAgentAPI.GetGameServerConnectionInfo();
            if (connectionInfo != null)
            {
                foreach (var p in connectionInfo.GamePortsConfiguration)
                {
                    portToUse = (ushort)p.ServerListeningPort;
                    Debug.LogFormat("Server listening port = {0}, client connection port = {1}",
                        p.ServerListeningPort, p.ClientConnectionPort);
                    break;
                }
            }

            utp.SetConnectionData("0.0.0.0", portToUse, "0.0.0.0");
        }

        NetworkManager.Singleton.StartServer();
    }

    private void OnClientConnected(ulong clientId)
    {
        string fakePlayFabId = clientId.ToString();
        OnPlayerAdded(fakePlayFabId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        string fakePlayFabId = clientId.ToString();
        OnPlayerRemoved(fakePlayFabId);
    }

    private void OnPlayerRemoved(string playfabId)
    {
        ConnectedPlayer player = _connectedPlayers.Find(
            x => x.PlayerId.Equals(playfabId, StringComparison.OrdinalIgnoreCase));
        if (player != null)
        {
            _connectedPlayers.Remove(player);
            PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
        }
        CheckPlayerCountToShutdown();
    }

    private void CheckPlayerCountToShutdown()
    {
        if (_connectedPlayers.Count <= 0)
        {
            StartShutdownProcess();
        }
    }

    private void OnPlayerAdded(string playfabId)
    {
        _connectedPlayers.Add(new ConnectedPlayer(playfabId));
        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
    }

    private void OnAgentError(string error)
    {
        Debug.Log(error);
    }

    private void OnShutdown()
    {
        StartShutdownProcess();
    }

    private void StartShutdownProcess()
    {
        Debug.Log("Server is shutting down");

        if (ServerNotification.Instance != null && NetworkManager.Singleton.IsServer)
        {
            ServerNotification.Instance.ShutdownClientRpc();
        }

        StartCoroutine(ShutdownServer());
    }

    IEnumerator ShutdownServer()
    {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }

    private void OnMaintenance(DateTime? NextScheduledMaintenanceUtc)
    {
        Debug.LogFormat("Maintenance scheduled for: {0}", NextScheduledMaintenanceUtc.Value.ToLongDateString());

        if (ServerNotification.Instance != null && NetworkManager.Singleton.IsServer)
        {
            long unixTime = ((DateTimeOffset)NextScheduledMaintenanceUtc.Value).ToUnixTimeSeconds();
            ServerNotification.Instance.MaintenanceClientRpc(unixTime);
        }
    }
}
