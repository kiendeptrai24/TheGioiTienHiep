using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.Multiplayer;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class PlayFabLobbyService : IDisposable
{
    private readonly Configuration _configuration;
    
    public Lobby CurrentLobby { get; private set; }
    public string CurrentLobbyId => CurrentLobby?.Id;
    public string CurrentConnectionString => CurrentLobby?.ConnectionString;
    public string CurrentServerIp { get; private set; }
    public ushort CurrentServerPort { get; private set; }
    public bool Initialized => _initialized;
    public bool Subscribed => _subscribed;

    public event Action<string> OnStatusChanged;
    public event Action<Lobby> OnLobbyReady;
    public event Action<string> OnLobbyJoinFailed;
    public event Action<string, ushort> OnServerInfoUpdated;
    public event Action<IList<LobbySearchResult>, PFEntityKey, int> OnLobbySearchLobbiesCompleted;

    private bool _initialized;
    private bool _subscribed;

    public PlayFabLobbyService()
    {
        _configuration = Configuration.Instance;
    }
    public bool HasLobby()
    {
        return CurrentLobby != null;
    }
    public void Initialize(PlayFabAuthenticationContext authenticationContext)
    {
        if (_initialized)
            return;

        PlayFabMultiplayer.Initialize();
        PlayFabMultiplayer.SetEntityToken(authenticationContext);
        _initialized = true;

        SubscribeEvents();
        Log("PlayFabMultiplayer initialized.");
    }

    /// <summary>
    /// Gọi ngay sau khi login PlayFab thành công.
    /// authContext lấy từ LoginResult.AuthenticationContext
    /// </summary>
    public void SetEntityToken(PlayFabAuthenticationContext authContext)
    {
        if (authContext == null)
        {
            Fail("AuthenticationContext is null.");
            return;
        }

        if (!_initialized)
            Initialize(authContext);

        PlayFabMultiplayer.SetEntityToken(authContext);
        Log("Entity token set for PlayFab Multiplayer.");
    }

    public void CreateLobbyAndJoin(
        PlayFabAuthenticationContext authContext,
        int maxPlayers = 4,
        string ip = "",
        ushort port = 0)
    {
        if(!_initialized) return; 
        if (authContext == null)
        {
            Fail("AuthenticationContext is null.");
            return;
        }


        // Đảm bảo Multiplayer SDK có token hợp lệ

        var createConfig = new LobbyCreateConfiguration
        {
            MaxMemberCount = (uint)Mathf.Clamp(maxPlayers, 2, 128),
            OwnerMigrationPolicy = LobbyOwnerMigrationPolicy.Automatic,
            AccessPolicy = LobbyAccessPolicy.Public
        };

        createConfig.LobbyProperties["serverIp"] = ip;
        createConfig.LobbyProperties["serverPort"] = port.ToString();
        createConfig.LobbyProperties["status"] = "waiting_server";

        var joinConfig = new LobbyJoinConfiguration();
        CurrentLobby = PlayFabMultiplayer.CreateAndJoinLobby(
            authContext,
            createConfig,
            joinConfig);

        Log("CreateAndJoinLobby requested...");
    }

    public void JoinLobby(
        PlayFabAuthenticationContext authContext,
        string connectionString)
    {
        if(!_initialized) return; 
        if (authContext == null)
        {
            Fail("AuthenticationContext is null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Fail("ConnectionString is empty.");
            return;
        }

        PlayFabMultiplayer.SetEntityToken(authContext);

        CurrentLobby = PlayFabMultiplayer.JoinLobby(
            authContext,
            connectionString,
            null);

        Log("JoinLobby requested...");
    }

    /// <summary>
    /// Chỉ owner mới update được LobbyProperties.
    /// Gọi sau khi dedicated server đã có IP/Port.
    /// </summary>
    public void UpdateServerInfo(
        PlayFabAuthenticationContext authContext,
        string serverIp,
        ushort serverPort)
    {
        if (CurrentLobby == null)
        {
            Fail("CurrentLobby is null.");
            return;
        }

        if (authContext == null)
        {
            Fail("AuthenticationContext is null.");
            return;
        }

        var update = new LobbyDataUpdate();
        update.LobbyProperties["serverIp"] = serverIp;
        update.LobbyProperties["serverPort"] = serverPort.ToString();
        update.LobbyProperties["status"] = "server_ready";

        CurrentLobby.PostUpdate(authContext, update);

        Log($"PostUpdate requested: {serverIp}:{serverPort}");
    }

    public bool TryGetServerInfo(out string ip, out ushort port)
    {
        ip = null;
        port = 0;

        if (CurrentLobby == null)
            return false;

        var props = CurrentLobby.GetLobbyProperties();
        if (props == null)
            return false;

        if (props.TryGetValue("serverIp", out var serverIp))
            ip = serverIp;

        if (props.TryGetValue("serverPort", out var serverPortStr))
            ushort.TryParse(serverPortStr, out port);

        return !string.IsNullOrWhiteSpace(ip) && port > 0;
    }

    public bool ApplyServerInfoToNgo()
    {
        if (!TryGetServerInfo(out var ip, out var port))
        {
            Fail("Lobby does not have valid serverIp/serverPort.");
            return false;
        }

        var utp = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (utp == null)
        {
            Fail("UnityTransport not found on NetworkManager.");
            return false;
        }

        CurrentServerIp = ip;
        CurrentServerPort = port;

        _configuration.ipAddress = ip;
        _configuration.port = port;

        utp.SetConnectionData(ip, port);

        Log($"NGO transport set to {ip}:{port}");
        return true;
    }

    public void LeaveAllLocalUsers()
    {
        if (CurrentLobby == null)
            return;

        CurrentLobby.LeaveAllLocalUsers();
        Log("LeaveAllLocalUsers requested...");
    }

    public void Dispose()
    {
        UnsubscribeEvents();

        if (_initialized)
        {
            PlayFabMultiplayer.Uninitialize();
            _initialized = false;
        }
    }

    private void SubscribeEvents()
    {
        if (_subscribed)
            return;

        PlayFabMultiplayer.OnLobbyCreateAndJoinCompleted += OnLobbyCreateAndJoinCompleted;
        PlayFabMultiplayer.OnLobbyJoinCompleted += OnLobbyJoinCompleted;
        PlayFabMultiplayer.OnLobbyUpdated += OnLobbyUpdated;
        PlayFabMultiplayer.OnLobbyPostUpdateCompleted += OnLobbyPostUpdateCompleted;
        PlayFabMultiplayer.OnLobbyFindLobbiesCompleted += OnLobbyFindLobbiesCompleted;
        PlayFabMultiplayer.OnLobbyDisconnected += OnLobbyDisconnected;
        PlayFabMultiplayer.OnError += OnError;

        _subscribed = true;
    }

    private void OnLobbyUpdated(Lobby lobby, bool ownerUpdated, bool maxMembersUpdated, bool accessPolicyUpdated, bool membershipLockUpdated, IList<string> updatedSearchPropertyKeys, IList<string> updatedLobbyPropertyKeys, IList<LobbyMemberUpdateSummary> memberUpdates)
    {
        if (!ReferenceEquals(CurrentLobby, lobby))
            return;

        SyncServerInfoFromLobby();
        Log("Lobby updated.");
    }

    private void UnsubscribeEvents()
    {
        if (!_subscribed)
            return;

        PlayFabMultiplayer.OnLobbyCreateAndJoinCompleted -= OnLobbyCreateAndJoinCompleted;
        PlayFabMultiplayer.OnLobbyJoinCompleted -= OnLobbyJoinCompleted;
        PlayFabMultiplayer.OnLobbyUpdated -= OnLobbyUpdated;
        PlayFabMultiplayer.OnLobbyPostUpdateCompleted -= OnLobbyPostUpdateCompleted;
        PlayFabMultiplayer.OnLobbyFindLobbiesCompleted -= OnLobbyFindLobbiesCompleted;
        PlayFabMultiplayer.OnLobbyDisconnected -= OnLobbyDisconnected;
        PlayFabMultiplayer.OnError -= OnError;

        _subscribed = false;
    }

    private void OnLobbyFindLobbiesCompleted(IList<LobbySearchResult> searchResults, PFEntityKey searchingEntity, int result)
    {
        OnLobbySearchLobbiesCompleted?.Invoke(searchResults, searchingEntity, result);
    }

    private void OnLobbyCreateAndJoinCompleted(Lobby lobby, int result)
    {
        if (!ReferenceEquals(CurrentLobby, lobby))
            CurrentLobby = lobby;

        if (LobbyError.SUCCEEDED(result))
        {
            SyncServerInfoFromLobby();
            Log($"CreateAndJoin success | LobbyId={lobby.Id} | ConnectionString={lobby.ConnectionString}");
            OnLobbyReady?.Invoke(lobby);
        }
        else
        {
            Fail($"CreateAndJoin failed. Result={result}");
        }
    }

    private void OnLobbyJoinCompleted(Lobby lobby, PFEntityKey newMember, int result)
    {
        if (!ReferenceEquals(CurrentLobby, lobby))
            CurrentLobby = lobby;
        if (LobbyError.SUCCEEDED(result))
        {
            SyncServerInfoFromLobby();
            Log($"JoinLobby success | LobbyId={lobby.Id}");
            OnLobbyReady?.Invoke(lobby);
        }
        else
        {
            Fail($"JoinLobby failed. Result={result}");
            OnLobbyJoinFailed?.Invoke($"JoinLobby failed. Result={result}");
        }
    }

    private void OnLobbyPostUpdateCompleted(Lobby lobby, PFEntityKey localUser, int result)
    {
        if (!ReferenceEquals(CurrentLobby, lobby))
            return;

        if (LobbyError.SUCCEEDED(result))
        {
            Log("Lobby PostUpdate completed successfully.");
        }
        else
        {
            Fail($"Lobby PostUpdate failed. Result={result}");
        }
    }

    private void OnLobbyDisconnected(Lobby lobby)
    {
        if (!ReferenceEquals(CurrentLobby, lobby))
            return;

        Log("Disconnected from lobby.");
    }

    private void OnError(PlayFabMultiplayerErrorArgs args)
    {
        Fail($"PlayFabMultiplayer error: {args.Message}");
    }

    private void SyncServerInfoFromLobby()
    {
        if (CurrentLobby == null || CurrentLobby.GetLobbyProperties() == null)
            return;

        if (CurrentLobby.GetLobbyProperties().TryGetValue("serverIp", out var ip))
            CurrentServerIp = ip;

        if (CurrentLobby.GetLobbyProperties().TryGetValue("serverPort", out var portStr) &&
            ushort.TryParse(portStr, out var port))
        {
            CurrentServerPort = port;
        }
        Debug.Log($"SyncServerInfoFromLobby: {CurrentServerIp}:{CurrentServerPort}");
        if (!string.IsNullOrWhiteSpace(CurrentServerIp) && CurrentServerPort > 0)
        {
            OnServerInfoUpdated?.Invoke(CurrentServerIp, CurrentServerPort);
        }
    }
    public void FindAllLobbies(PlayFabAuthenticationContext authContext)
    {
        if(!_initialized) return;
        if(authContext == null)
        {
            Fail("AuthenticationContext is null.");
            return;
        }
        PFEntityKey entityKey = new PFEntityKey(authContext);
        LobbySearchConfiguration config = new LobbySearchConfiguration();
        PlayFabMultiplayer.FindLobbies(entityKey, config);
    }
    private void Log(string message)
    {
        Debug.Log($"[PlayFabLobbyService] {message}");
        OnStatusChanged?.Invoke(message);
    }

    private void Fail(string message)
    {
        Debug.LogError($"[PlayFabLobbyService] {message}");
        OnStatusChanged?.Invoke(message);
    }
}