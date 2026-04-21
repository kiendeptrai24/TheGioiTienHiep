using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.Multiplayer;
using Unity.Netcode;
using UnityEngine;

public class LobbyController : Singleton<LobbyController>
{
    private PlayFabLobbyService _lobbyService;
    private PlayFabAuthenticationContext authContext;
    private List<LobbySearchResult> lobbies = new List<LobbySearchResult>();
    public List<LobbySearchResult> GetLobbies() => lobbies;
    public event Action<bool, LobbySearchResult> OnLobbySearchLobbiesCompleted;
    private PlayfabDataManager playfabDataManager;

    protected override void Awake()
    {
        playfabDataManager = PlayfabDataManager.Instance;
        playfabDataManager.LoginSuccess += OnLoginSuccess;
        _lobbyService = new PlayFabLobbyService();

        _lobbyService.OnStatusChanged += msg =>
        {
            Debug.Log(msg);
        };
        _lobbyService.OnLobbyReady += lobby =>
        {
            Debug.Log("Lobby ready: " + lobby.Id);
            Debug.Log("ConnectionString: " + lobby.ConnectionString);
            ConnectNgo();
        };
        _lobbyService.OnServerInfoUpdated += (ip, port) =>
        {
            Debug.Log($"Server info from lobby: {ip}:{port}");
        };
        _lobbyService.OnLobbySearchLobbiesCompleted += (lobbies, entityKey, result) =>
        {
            foreach (var lobby in lobbies)
            {
                Debug.Log(lobby.LobbyId + lobby.ConnectionString);
                if (lobby.SearchProperties.TryGetValue("serverIp", out var ip))
                {
                    Debug.Log("serverIp: " + ip);
                }

                if (lobby.SearchProperties.TryGetValue("serverPort", out var port))
                {
                    Debug.Log("serverPort: " + port);
                }
                if (CheckLobbyCanJoin(lobby))
                    lobbies.Add(lobby);
            }
            if (lobbies.Count > 0)
                OnLobbySearchLobbiesCompleted?.Invoke(true, lobbies[0]);
            else
                OnLobbySearchLobbiesCompleted?.Invoke(false, null);
        };

    }

    private void OnLoginSuccess(AuthResult result)
    {
        authContext = result.clientApi.authenticationContext;
        _lobbyService.Initialize(result.clientApi.authenticationContext);
    }

    public bool CheckLobbyCanJoin(LobbySearchResult lobby)
    {
        if (lobby == null) return false;

        bool hasRoom = lobby.CurrentMemberCount < lobby.MaxMemberCount;
        bool unlocked = lobby.MembershipLock == LobbyMembershipLock.Unlocked;
        bool hasIpAndPort = lobby.SearchProperties.TryGetValue("serverIp", out var ip) &&
            lobby.SearchProperties.TryGetValue("serverPort", out var port);

        return hasRoom && unlocked && hasIpAndPort;
    }
    public void CreateLobby(PlayFabAuthenticationContext authContext, string ip = "", ushort port = 0)
    {
        _lobbyService.CreateLobbyAndJoin(authContext, 100, ip, port);
    }
    public void JoinLobby(PlayFabAuthenticationContext authContext, string connectionString)
    {
        _lobbyService.JoinLobby(authContext, connectionString);
    }
    public bool HasLobby()
    {
        return _lobbyService.HasLobby();
    }
    public void UpdateLobbyServer(PlayFabAuthenticationContext authContext, string ip, ushort port)
    {
        _lobbyService.UpdateServerInfo(authContext, ip, port);
    }
    [ContextMenu("Update Lobby Server")]
    public void UpdateServerInfo()
    {
        UpdateLobbyServer(authContext, "127.0.0.1", 0);
    }
    public void GetLobbyServer(PlayFabAuthenticationContext authContext)
    {
        _lobbyService.FindAllLobbies(authContext);
    }
    [ContextMenu("Get Lobby Server")]
    public void GetLobbyServer()
    {
        _lobbyService.FindAllLobbies(authContext);
    }
    public void ConnectNgo()
    {
        _lobbyService.ApplyServerInfoToNgo();
    }

    private void OnDestroy()
    {
        _lobbyService?.Dispose();
    }
}