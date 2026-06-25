using System;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;

public class PlayfabDataManager : Singleton<PlayfabDataManager>
{
    private const float SessionHeartbeatIntervalSeconds = 20f;
    private const float SessionRetryIntervalSeconds = 2f;
    private const int MaxSessionRetryAttempts = 30;
    private const string CreateAccountScreenName = "CreateAccount";
    private const string CreateCharacterPanelName = "Panel (CreateNv)";

    public ActionNavigationSpecificScreen navigationToCharacterSelectionScreen;

    public event Action<GameData> OnLoadGameFormPlayfab;
    public event Action<List<ItemData>> OnLoadCharacterFormPlayfab;
    public event Action<List<ItemData>> OnCharacterChanged;
    public event Action<AuthResult> LoginSuccess;
    public event Action<AuthError> LoginError;
    public event Action<string> LoginStatusChanged;

    [SerializeField] private GameData gameData = new GameData();

    private PlayfabSessionState sessionState;
    private PlayfabAuthSessionService authSessionService;
    private PlayfabRemoteGameDataService remoteGameDataService;
    private PlayfabClientRuntimeService clientRuntimeService;
    private GameDataCenterManager gameDataCenterManager;
    private bool isApplicationQuitting;

    private AuthResult _pendingAuthResult;
    private Coroutine _sessionRetryCoroutine;

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
        CancelSessionRetry();
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

    public void SaveGameData(Action<bool> onCompleted = null)
    {
        remoteGameDataService.SaveGameData(onCompleted);
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
        _pendingAuthResult = result;

        authSessionService.AcquireRealtimeSession(result, sessionResult =>
        {
            _pendingAuthResult = null;
            StartSessionHeartbeat();
            LoginSuccess?.Invoke(sessionResult);

            if (gameDataCenterManager.IsReady())
            {
                gameData.Clear();
                LoadCharacterDataChoose();
            }
        }, HandleLoginError, HandleSessionWaiting);
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
        authSessionService.MarkLoggedOutLocally();
        LoginStatusChanged?.Invoke("Phat hien dang nhap o thiet bi khac. Dang luu du lieu truoc khi dang xuat...");
        SaveBeforeRemoteKick(PostRemoteKick);
    }

    private void PostRemoteKick(bool saveSucceeded)
    {
        if (!saveSucceeded)
        {
            CancelInvoke(nameof(RefreshRealtimeSessionLock));
            authSessionService.Logout(_ => CompleteRemoteKick(false), _ => CompleteRemoteKick(false));
            return;
        }

        ReleaseRealtimeSessionLock(() =>
        {
            authSessionService.Logout(_ => CompleteRemoteKick(true), _ => CompleteRemoteKick(true));
        });
    }

    private void CompleteRemoteKick(bool saveSucceeded)
    {
        ResetLocalSessionState();
        clientRuntimeService.ShutdownNetworkIfNeeded();
        clientRuntimeService.ResetClientSystems();

        var message = saveSucceeded
            ? "Tai khoan nay vua dang nhap o thiet bi khac."
            : "Tai khoan nay vua dang nhap o thiet bi khac. Da co loi khi luu du lieu truoc khi dang xuat.";
        LoginError?.Invoke(new AuthError("SESSION_REVOKED", message));
    }

    private void SaveBeforeRemoteKick(Action<bool> onCompleted)
    {
        var saveManager = SaveLoadManager.Instance != null ? SaveLoadManager.Instance.saveManager as SaveLoadPlayfab : null;
        if (saveManager == null)
        {
            Debug.LogWarning("[PlayfabDataManager] SaveLoadPlayfab not found. Releasing session without save confirmation.");
            onCompleted?.Invoke(false);
            return;
        }

        saveManager.SaveGame(success =>
        {
            if (!success)
            {
                Debug.LogWarning("[PlayfabDataManager] Remote save failed during forced logout.");
            }

            onCompleted?.Invoke(success);
        });
    }

    private void HandleSessionWaiting(CloudSessionRequestResult sessionResult)
    {
        LoginStatusChanged?.Invoke(string.IsNullOrEmpty(sessionResult.message)
            ? "Dang dong bo du lieu va dang xuat phien cu..."
            : sessionResult.message);
        Debug.Log($"[PlayfabDataManager] Phien dang nhap khac dang hoat dong (session: {sessionResult.previousSessionId}). Dang cho giai phong...");

        if (_sessionRetryCoroutine != null)
        {
            StopCoroutine(_sessionRetryCoroutine);
        }

        _sessionRetryCoroutine = StartCoroutine(SessionRetryRoutine());
    }

    private System.Collections.IEnumerator SessionRetryRoutine()
    {
        var authResult = _pendingAuthResult;
        if (authResult == null)
        {
            Debug.LogError("[PlayfabDataManager] SessionRetryRoutine: No pending auth result!");
            yield break;
        }

        var wait = new WaitForSeconds(SessionRetryIntervalSeconds);

        for (int attempt = 1; attempt <= MaxSessionRetryAttempts; attempt++)
        {
            Debug.Log($"[PlayfabDataManager] Thu lai lay session... (lan {attempt}/{MaxSessionRetryAttempts})");
            yield return wait;

            if (isApplicationQuitting)
            {
                yield break;
            }

            bool completed = false;
            bool succeeded = false;
            AuthError fatalError = null;

            authSessionService.RetryAcquireSession(authResult, result =>
            {
                succeeded = true;
                completed = true;
                _pendingAuthResult = null;
                _sessionRetryCoroutine = null;

                Debug.Log("[PlayfabDataManager] Session da san sang, dang nhap thanh cong!");
                StartSessionHeartbeat();
                LoginSuccess?.Invoke(result);

                if (gameDataCenterManager.IsReady())
                {
                    gameData.Clear();
                    LoadCharacterDataChoose();
                }
            }, error =>
            {
                completed = true;
                // If still waiting, continue the loop. Other errors will break.
                if (error.code != "SESSION_STILL_WAITING")
                {
                    fatalError = error;
                    Debug.LogError($"[PlayfabDataManager] Loi khi thu lai session: {error.message}");
                }
            });

            // Wait for the async call to complete
            yield return new WaitUntil(() => completed);

            if (succeeded)
            {
                yield break;
            }

            if (fatalError != null)
            {
                _pendingAuthResult = null;
                _sessionRetryCoroutine = null;
                ResetLocalSessionState();
                LoginError?.Invoke(fatalError);
                yield break;
            }
        }

        // Max retries reached
        Debug.LogError("[PlayfabDataManager] Het thoi gian cho session. Dang nhap that bai.");
        _pendingAuthResult = null;
        _sessionRetryCoroutine = null;
        ResetLocalSessionState();
        LoginError?.Invoke(new AuthError("SESSION_TIMEOUT", "Khong the lay session. Vui long thu lai sau."));
    }

    private void CancelSessionRetry()
    {
        if (_sessionRetryCoroutine != null)
        {
            StopCoroutine(_sessionRetryCoroutine);
            _sessionRetryCoroutine = null;
        }
        _pendingAuthResult = null;
    }
}
