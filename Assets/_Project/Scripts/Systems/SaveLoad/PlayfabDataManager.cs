using System;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;

public class PlayfabDataManager : Singleton<PlayfabDataManager>
{
    private const float SessionHeartbeatIntervalSeconds = 20f;
    private const string CreateAccountScreenName = "CreateAccount";
    private const string CreateCharacterPanelName = "Panel (CreateNv)";

    public ActionNavigationSpecificScreen navigationToCharacterSelectionScreen;

    public event Action<GameData> OnLoadGameFormPlayfab;
    public event Action<List<ItemData>> OnLoadCharacterFormPlayfab;
    public event Action<List<ItemData>> OnCharacterChanged;
    public event Action<AuthResult> LoginSuccess;
    public event Action<AuthError> LoginError;

    [SerializeField] private GameData gameData = new GameData();

    private PlayfabSessionState sessionState;
    private PlayfabAuthSessionService authSessionService;
    private PlayfabRemoteGameDataService remoteGameDataService;
    private PlayfabClientRuntimeService clientRuntimeService;
    private GameDataCenterManager gameDataCenterManager;
    private bool isApplicationQuitting;

    public bool ready => sessionState != null && sessionState.Ready;
    public bool IsAuthenticated => authSessionService != null && authSessionService.IsAuthenticated;

    public AuthFacade GetAuthManager() => authSessionService.AuthFacade;
    public PlayFabClientInstanceAPI GetClientAPI() => authSessionService.ClientApi;
    public List<ItemData> GetCharactersData() => remoteGameDataService.GetCharactersData();

    protected override void Awake()
    {
        base.Awake();

        sessionState = new PlayfabSessionState(gameData);
        authSessionService = new PlayfabAuthSessionService(sessionState);
        remoteGameDataService = new PlayfabRemoteGameDataService(sessionState);
        clientRuntimeService = new PlayfabClientRuntimeService();

        gameDataCenterManager = GameDataCenterManager.Instance;
        gameDataCenterManager.OnLoadGameDataCenterSuccessed += OnDataCenterReady;
        navigationToCharacterSelectionScreen = GetComponent<ActionNavigationSpecificScreen>();
    }

    protected override void Start()
    {
        base.Start();
        authSessionService.Configure();

        if (Configuration.Instance.startwithHost)
        {
            authSessionService.HostLogin(HandleLoginSuccess, HandleLoginError);
            return;
        }

        if (authSessionService.ShouldAutoLoginClient())
        {
            authSessionService.AutoLogin(HandleLoginSuccess, HandleLoginError);
        }
    }
    override protected void OnApplicationQuit()
    {
        ShutDownPlayfab();
        base.OnApplicationQuit();
    }
    public void ShutDownPlayfab()
    {
        isApplicationQuitting = true;
        MarkLoggedOutLocally();
        ReleaseRealtimeSessionLock();
    }

    protected override void OnDestroy()
    {
        if (gameDataCenterManager != null)
        {
            gameDataCenterManager.OnLoadGameDataCenterSuccessed -= OnDataCenterReady;
        }

        if (isApplicationQuitting)
        {
            base.OnDestroy();
            return;
        }
        base.OnDestroy();
    }

    public void Login(LoginData loginData)
    {
        authSessionService.Login(loginData, HandleLoginSuccess, HandleLoginError);
    }

    public void Logout()
    {
        BeginSessionExit(() =>
        {
            authSessionService.Logout(_ => CompleteLogout(), _ => CompleteLogout());
        });
    }

    public void ChangeAccount()
    {
        BeginSessionExit(() =>
        {
            SaveLoadManager.Instance.SaveGame();
            ResetLocalSessionState();
            clientRuntimeService.ShutdownNetworkIfNeeded();
            clientRuntimeService.ResetClientSystems();

            var createAccountScreen = ScreenManagerHub.Instance.Get(CreateAccountScreenName);
            createAccountScreen.NavigateTo(CreateCharacterPanelName);
        });
    }

    public void OnCharacterLoaded(string characterId)
    {
        if (!IsAuthenticated)
        {
            return;
        }

        StartSessionHeartbeat();
        remoteGameDataService.PrepareCharacterLoad(characterId);
        SceneLoadManager.Instance.LoadSceneLoading();

        remoteGameDataService.LoadGameData(loadedGameData =>
        {
            if (!IsAuthenticated)
            {
                return;
            }

            OnLoadGameFormPlayfab?.Invoke(loadedGameData);
            if (clientRuntimeService.TryStartNetworkSession())
            {
                SceneLoadManager.Instance.UnLoadScene("LoadingScene");
            }
        });
    }

    public void AddCharacter(ItemData itemCharacter)
    {
        remoteGameDataService.AddCharacter(itemCharacter, characters =>
        {
            OnCharacterChanged?.Invoke(characters);
        });
    }

    public void SaveGameData()
    {
        remoteGameDataService.SaveGameData();
    }

    public void onSuccess(AuthResult result)
    {
        HandleLoginSuccess(result);
    }

    public void onError(AuthError error)
    {
        HandleLoginError(error);
    }

    private void OnDataCenterReady(GameDataCenter center)
    {
        if (IsAuthenticated)
        {
            LoadCharacterDataChoose();
        }
    }

    private void HandleLoginSuccess(AuthResult result)
    {
        gameDataCenterManager.onSuccess(result.clientApi);

        authSessionService.AcquireRealtimeSession(result, sessionResult =>
        {
            LoginSuccess?.Invoke(sessionResult);

            if (gameDataCenterManager.IsReady())
            {
                gameData.Clear();
                LoadCharacterDataChoose();
            }
        }, HandleLoginError);
    }

    private void HandleLoginError(AuthError error)
    {
        if (!sessionState.HasLoggedIn && !sessionState.SessionLockAcquired)
        {
            ResetLocalSessionState();
        }

        LoginError?.Invoke(error);
    }

    private void LoadCharacterDataChoose()
    {
        if (!IsAuthenticated)
        {
            return;
        }

        remoteGameDataService.ConfigureRemoteServices();
        remoteGameDataService.LoadCharacterSelectionData(characters =>
        {
            if (!IsAuthenticated)
            {
                return;
            }

            OnLoadCharacterFormPlayfab?.Invoke(characters);
        });
    }

    private void StartSessionHeartbeat()
    {
        CancelInvoke(nameof(RefreshRealtimeSessionLock));
        RefreshRealtimeSessionLock();
        InvokeRepeating(nameof(RefreshRealtimeSessionLock), SessionHeartbeatIntervalSeconds, SessionHeartbeatIntervalSeconds);
    }

    private void RefreshRealtimeSessionLock()
    {
        authSessionService.RefreshRealtimeSessionLock(result =>
        {
            if (result != null && !result.valid && result.shouldLogout)
            {
                ForceLogoutFromRemoteSession();
            }
        }, error =>
        {
            Debug.LogWarning($"Refresh realtime session lock failed: {error.message}");
        });
    }

    private void BeginSessionExit(Action onReleased)
    {
        MarkLoggedOutLocally();
        ReleaseRealtimeSessionLock(onReleased);
    }

    private void ReleaseRealtimeSessionLock(Action onReleased = null)
    {
        CancelInvoke(nameof(RefreshRealtimeSessionLock));
        authSessionService.ReleaseRealtimeSessionLock(onReleased, error =>
        {
            Debug.LogWarning($"Release realtime session lock failed: {error.message}");
        });
    }

    private void CompleteLogout()
    {
        SaveLoadManager.Instance.SaveGame();
        ResetLocalSessionState();
        clientRuntimeService.ShutdownNetworkIfNeeded();
        clientRuntimeService.ResetClientSystems();
    }

    private void MarkLoggedOutLocally()
    {
        authSessionService.MarkLoggedOutLocally();
        CancelInvoke(nameof(RefreshRealtimeSessionLock));
    }

    private void ResetLocalSessionState()
    {
        authSessionService.ResetLocalSessionState();
        remoteGameDataService.ClearRemoteCache();
        clientRuntimeService.ClearBattleHistory();
    }

    private void ForceLogoutFromRemoteSession()
    {
        MarkLoggedOutLocally();
        authSessionService.Logout(_ => PostRemoteKick(), _ => PostRemoteKick());
    }

    private void PostRemoteKick()
    {
        SaveLoadManager.Instance.SaveGame();
        ResetLocalSessionState();
        clientRuntimeService.ShutdownNetworkIfNeeded();
        clientRuntimeService.ResetClientSystems();
        LoginError?.Invoke(new AuthError("SESSION_REVOKED", "Tai khoan nay vua dang nhap o thiet bi khac."));
    }
}
