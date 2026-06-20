#if ENABLE_PLAYFABSERVER_API
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;
using System.Collections;
using PlayFab.MultiplayerAgent.Model;
using PlayFab;

public class ServerStartUp : Singleton<ServerStartUp>
{
    public Configuration configuration;
    private List<ConnectedPlayer> _connectedPlayers;
    private UnityTransport transport;
    private bool isServerStart = false;
    protected override void Start()
    {
        if (configuration.IsServerRemoteBuild())
        {
            StartRemoteServer();
        }
    }
    public void StartServer()
    {
        if (isServerStart) return;
        if (configuration.startwithHost) return;
        if (configuration.IsServerBuild() == false) return;
        NetworkManager.Singleton.StartServer();
        isServerStart = true;
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

        if (isServerStart) return;
        NetworkManager.Singleton.StartServer();
        isServerStart = true;
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
    }
}
#endif