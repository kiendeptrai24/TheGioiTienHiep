using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

#if ENABLE_PLAYFABSERVER_API
using System.Collections.Generic;
using PlayFab;
using PlayFab.MultiplayerAgent.Model;
#endif

public class ServerStartUp : Singleton<ServerStartUp>
{
    public Configuration configuration;

    private bool isServerStart = false;
    private UnityTransport transport;

#if ENABLE_PLAYFABSERVER_API
    private List<ConnectedPlayer> _connectedPlayers;
#endif

    protected override void Start()
    {
        base.Start();

        if (!configuration.IsServerRemoteBuild())
        {
            return;
        }

        StartRemoteServer();
    }

    private void StartRemoteServer()
    {
        Debug.Log("[ServerStartUp] StartRemoteServer");

        transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        if (IsRunningOnPlayFabMps())
        {
            StartPlayFabServer();
        }
        else
        {
            StartLinuxEC2Server();
        }
    }

    private bool IsRunningOnPlayFabMps()
    {
        // Chỉ PlayFab Multiplayer Server mới có biến này.
        string gsdkConfigFile = Environment.GetEnvironmentVariable("GSDK_CONFIG_FILE");

        return !string.IsNullOrEmpty(gsdkConfigFile);
    }

    private void StartLinuxEC2Server()
    {
        Debug.Log("[ServerStartUp] Running as Linux EC2 standalone server.");

        ushort port = GetPortFromCommandLine(configuration.port);

        if (transport != null)
        {
            transport.SetConnectionData("0.0.0.0", port, "0.0.0.0");
            Debug.Log($"[ServerStartUp] EC2 server listening on port: {port}");
        }
        else
        {
            Debug.LogError("[ServerStartUp] UnityTransport not found.");
        }

        StartServer();
    }

#if ENABLE_PLAYFABSERVER_API
    private void StartPlayFabServer()
    {
        Debug.Log("[ServerStartUp] Running inside PlayFab Multiplayer Server.");

        _connectedPlayers = new List<ConnectedPlayer>();

        PlayFabMultiplayerAgentAPI.IsDebugging = configuration.playFabDebugging;

        PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
        PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
        PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
        PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

        PlayFabMultiplayerAgentAPI.Start();

        StartCoroutine(ReadyForPlayers());
    }

    private IEnumerator ReadyForPlayers()
    {
        yield return new WaitForSeconds(0.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    }

    private void OnServerActive()
    {
        Debug.Log("[ServerStartUp] Server Started From PlayFab Agent Activation");

        ushort portToUse = configuration.port;

        var connectionInfo = PlayFabMultiplayerAgentAPI.GetGameServerConnectionInfo();

        if (connectionInfo != null && connectionInfo.GamePortsConfiguration != null)
        {
            foreach (var p in connectionInfo.GamePortsConfiguration)
            {
                portToUse = (ushort)p.ServerListeningPort;

                Debug.LogFormat(
                    "[ServerStartUp] Server listening port = {0}, client connection port = {1}",
                    p.ServerListeningPort,
                    p.ClientConnectionPort
                );

                break;
            }
        }

        if (transport != null)
        {
            transport.SetConnectionData("0.0.0.0", portToUse, "0.0.0.0");
        }

        StartServer();
    }
#else
    private void StartPlayFabServer()
    {
        Debug.LogError("[ServerStartUp] PlayFab server API is not enabled.");
    }
#endif

    public void StartServer()
    {
        if (isServerStart)
        {
            return;
        }

        if (configuration.startwithHost)
        {
            return;
        }

        if (!configuration.IsServerBuild())
        {
            return;
        }

        bool result = NetworkManager.Singleton.StartServer();

        Debug.Log($"[ServerStartUp] NetworkManager StartServer result: {result}");

        isServerStart = true;
    }

    private ushort GetPortFromCommandLine(ushort defaultPort)
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-port" && i + 1 < args.Length)
            {
                if (ushort.TryParse(args[i + 1], out ushort port))
                {
                    return port;
                }
            }
        }

        return defaultPort;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[ServerStartUp] Client connected: {clientId}");

#if ENABLE_PLAYFABSERVER_API
        if (IsRunningOnPlayFabMps())
        {
            string fakePlayFabId = clientId.ToString();
            OnPlayerAdded(fakePlayFabId);
        }
#endif
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[ServerStartUp] Client disconnected: {clientId}");

#if ENABLE_PLAYFABSERVER_API
        if (IsRunningOnPlayFabMps())
        {
            string fakePlayFabId = clientId.ToString();
            OnPlayerRemoved(fakePlayFabId);
        }
#endif
    }

#if ENABLE_PLAYFABSERVER_API
    private void OnPlayerAdded(string playfabId)
    {
        if (_connectedPlayers == null)
        {
            return;
        }

        _connectedPlayers.Add(new ConnectedPlayer(playfabId));

        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
    }

    private void OnPlayerRemoved(string playfabId)
    {
        if (_connectedPlayers == null)
        {
            return;
        }

        ConnectedPlayer player = _connectedPlayers.Find(
            x => x.PlayerId.Equals(playfabId, StringComparison.OrdinalIgnoreCase)
        );

        if (player != null)
        {
            _connectedPlayers.Remove(player);
            PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
        }
    }

    private void OnAgentError(string error)
    {
        Debug.LogError($"[ServerStartUp] PlayFab Agent Error: {error}");
    }

    private void OnShutdown()
    {
        StartShutdownProcess();
    }

    private void OnMaintenance(DateTime? nextScheduledMaintenanceUtc)
    {
        if (nextScheduledMaintenanceUtc.HasValue)
        {
            Debug.Log($"[ServerStartUp] Maintenance scheduled for: {nextScheduledMaintenanceUtc.Value}");
        }
    }
#endif

    private void StartShutdownProcess()
    {
        Debug.Log("[ServerStartUp] Server is shutting down");
        StartCoroutine(ShutdownServer());
    }

    private IEnumerator ShutdownServer()
    {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

#if ENABLE_PLAYFABSERVER_API
        if (IsRunningOnPlayFabMps())
        {
            PlayFabMultiplayerAgentAPI.OnMaintenanceCallback -= OnMaintenance;
            PlayFabMultiplayerAgentAPI.OnShutDownCallback -= OnShutdown;
            PlayFabMultiplayerAgentAPI.OnServerActiveCallback -= OnServerActive;
            PlayFabMultiplayerAgentAPI.OnAgentErrorCallback -= OnAgentError;
        }
#endif
    }
}