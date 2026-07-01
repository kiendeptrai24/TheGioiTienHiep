using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using Unity.Netcode;
using UnityEngine;

public class PlayfabDataManager : Singleton<PlayfabDataManager>
{
    // Heartbeat mỗi 2 giây
    private const float HeartbeatIntervalSeconds = 2f;
    // Thời gian chờ khi session trước còn online, rồi retry
    private const float SessionWaitBeforeRetrySeconds = 3f;
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
    private bool _isWaitingForGameplayConnection;
    private bool _isHandlingNetworkDisconnect;

    private Coroutine _heartbeatCoroutine;
    private bool _isChangingAccount;
    private bool _isAutoLoginInProgress;
    private bool _currentLoginIsAuto;

    public bool ready => sessionState != null && sessionState.Ready;
    public bool IsAuthenticated => authSessionService != null && authSessionService.IsAuthenticated;
    public bool IsChangingAccount => _isChangingAccount;
    public bool IsAutoLoginInProgress => _isAutoLoginInProgress;
    public bool CurrentLoginIsAuto => _currentLoginIsAuto;

    public AuthFacade GetAuthManager() => authSessionService.AuthFacade;
    public PlayFabClientInstanceAPI GetClientAPI() => authSessionService.ClientApi;
    public List<ItemData> GetCharactersData() => remoteGameDataService.GetCharactersData();
    private AuthResult authResult;

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
        RegisterNetworkCallbacks();
    }

    protected override void Start()
    {
        base.Start();
        authSessionService.Configure();

        if (Configuration.Instance.startwithHost)
        {
            BeginLoginFlow(false);
            authSessionService.HostLogin(HandleAuthSuccess, HandleLoginError);
            return;
        }

        if (authSessionService.ShouldAutoLoginClient())
        {
            BeginLoginFlow(true);
            authSessionService.AutoLogin(HandleAuthSuccess, HandleLoginError);
        }
    }
    override protected void OnApplicationQuit()
    {
        ShutDownPlayfab();
        base.OnApplicationQuit();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause) ShutDownPlayfab();
    }
    public void ShutDownPlayfab()
    {
        isApplicationQuitting = true;
        StopHeartbeat();
        authSessionService.MarkLoggedOutLocally();
    }

    protected override void OnDestroy()
    {
        StopHeartbeat();

        if (gameDataCenterManager != null)
        {
            gameDataCenterManager.OnLoadGameDataCenterSuccessed -= OnDataCenterReady;
        }

        UnregisterNetworkCallbacks();

        if (isApplicationQuitting)
        {
            base.OnDestroy();
            return;
        }
        base.OnDestroy();
    }

    public void Login(LoginData loginData)
    {
        BeginLoginFlow(false);
        authSessionService.Login(loginData, HandleAuthSuccess, HandleLoginError);
    }

    public void Logout()
    {
        StopHeartbeat();
        authSessionService.LogoutSession(() =>
            authSessionService.Logout(_ => CompleteLogout(), _ => CompleteLogout()));
    }

    public void ChangeAccount()
    {
        if (!IsAuthenticated)
        {
            return;
        }

        _isChangingAccount = true;
        var saveManager = SaveLoadManager.Instance != null ? SaveLoadManager.Instance.saveManager as SaveLoadPlayfab : null;
        if (saveManager != null)
        {
            saveManager.SaveGame(_ => CompleteCharacterSwitch());
            return;
        }

        CompleteCharacterSwitch();
    }

    public void OnCharacterLoaded(string characterId)
    {
        if (!IsAuthenticated)
        {
            return;
        }

        StartHeartbeat();
        remoteGameDataService.PrepareCharacterLoad(characterId);
        SceneLoadManager.Instance.LoadSceneLoading();
        RegisterNetworkCallbacks();
        _isWaitingForGameplayConnection = false;

        remoteGameDataService.LoadGameData(loadedGameData =>
        {
            if (!IsAuthenticated)
            {
                return;
            }

            OnLoadGameFormPlayfab?.Invoke(loadedGameData);
            if (clientRuntimeService.TryStartNetworkSession())
            {
                _isWaitingForGameplayConnection = true;
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
        authResult = result;
        HandleAuthSuccess(result);
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

    // ── Auth → Tạo session ─────────────────────────────────────────────────────

    // Gọi sau khi PlayFab auth thành công: dừng heartbeat cũ trước, sau đó tạo session mới.
    // StopHeartbeat() ở đây tránh heartbeat phiên cũ giữ lastHeartbeat tươi và tự block CreateSession.
    private void HandleAuthSuccess(AuthResult result)
    {
        EndLoginFlow();
        StopHeartbeat(); // dừng heartbeat cũ trước khi tạo session mới
        gameDataCenterManager.onSuccess(result.clientApi);
        authSessionService.CreateSession(false, OnSessionCreateSuccess, OnSessionCreateError);
    }

    private void OnSessionCreateSuccess(SessionCreateResponse response)
    {
        StartHeartbeat();

        var authResult = new AuthResult
        {
            userId = sessionState.CurrentPlayFabId,
            sessionId = sessionState.SessionId,
            shouldWaitBeforeEnter = response != null && response.shouldWait,
            waitBeforeEnterSeconds = SessionWaitBeforeRetrySeconds,
            message = "Đăng nhập thành công."
        };
        LoginStatusChanged?.Invoke(authResult.message);
        if (response != null && response.shouldWait && !string.IsNullOrEmpty(response.message))
        {
            LoginStatusChanged?.Invoke(response.message);
        }
        LoginSuccess?.Invoke(authResult);

        if (gameDataCenterManager.IsReady())
        {
            if (_isChangingAccount)
            {
                _isChangingAccount = false;
                gameData.ClearNotCharacterData();
            }
            else
            {
                gameData.Clear();
            }
            LoadCharacterDataChoose();
        }
    }

    private void OnSessionCreateError(AuthError error)
    {
        ResetLocalSessionState();
        LoginError?.Invoke(error);
    }

    // ── Heartbeat mỗi 2 giây ──────────────────────────────────────────────────

    // Đảm bảo chỉ 1 coroutine heartbeat chạy tại một thời điểm.
    private void StartHeartbeat()
    {
        StopHeartbeat();
        _heartbeatCoroutine = StartCoroutine(HeartbeatRoutine());
    }

    private void StopHeartbeat()
    {
        if (_heartbeatCoroutine != null)
        {
            StopCoroutine(_heartbeatCoroutine);
            _heartbeatCoroutine = null;
        }
    }

    private IEnumerator HeartbeatRoutine()
    {
        var wait = new WaitForSeconds(HeartbeatIntervalSeconds);

        while (sessionState.IsAuthenticated && !isApplicationQuitting)
        {
            // Gửi heartbeat NGAY (kiểm tra session trước khi chờ).
            // Lần đầu chạy: xác minh session vừa tạo còn hợp lệ không.
            // Nếu thiết bị khác đã ghi đè sessionId → bị kick ngay, không chờ 2 giây.
            bool done = false;

            authSessionService.SendHeartbeat(
                response =>
                {
                    done = true;
                    if (response != null && response.shouldLogout)
                    {
                        Debug.LogWarning($"[PlayfabDataManager] Session bị kick: {response.reason}");
                        HandleSessionInvalid();
                    }
                },
                error =>
                {
                    done = true;
                    // Chỉ log, không crash. Vòng tiếp theo sẽ thử lại.
                    Debug.LogWarning($"[PlayfabDataManager] Heartbeat lỗi: {error.message}");
                });

            yield return new WaitUntil(() => done);

            if (!sessionState.IsAuthenticated || isApplicationQuitting) yield break;

            // Chờ 2 giây SAU khi heartbeat xong
            yield return wait;
        }
    }

    // ── Kick user khi session không hợp lệ ───────────────────────────────────

    private void HandleSessionInvalid()
    {
        StopHeartbeat();
        authSessionService.MarkLoggedOutLocally();
        LoginStatusChanged?.Invoke("Tài khoản của bạn đã được đăng nhập ở nơi khác.");

        var saveManager = SaveLoadManager.Instance != null
            ? SaveLoadManager.Instance.saveManager as SaveLoadPlayfab
            : null;

        if (saveManager == null)
        {
            CompleteKick(false);
            return;
        }

        saveManager.SaveGame(success =>
        {
            if (!success) Debug.LogWarning("[PlayfabDataManager] Save failed during remote kick.");
            CompleteKick(success);
        });
    }

    private void CompleteKick(bool savedOk)
    {
        authSessionService.LogoutSession(() =>
        {
            ResetLocalSessionState();
            clientRuntimeService.ShutdownNetworkIfNeeded();
            clientRuntimeService.ResetClientSystems();

            LoginError?.Invoke(new AuthError("SESSION_REVOKED",
                savedOk
                    ? "Tài khoản của bạn đã được đăng nhập ở nơi khác."
                    : "Tài khoản của bạn đã được đăng nhập ở nơi khác. Có lỗi khi lưu dữ liệu."));
        });
    }

    // ── Xử lý lỗi login ───────────────────────────────────────────────────────

    private void HandleLoginError(AuthError error)
    {
        EndLoginFlow();
        if (!sessionState.HasLoggedIn)
        {
            ResetLocalSessionState();
        }
        LoginError?.Invoke(error);
    }

    // ── Load dữ liệu ──────────────────────────────────────────────────────────

    private void LoadCharacterDataChoose()
    {
        if (!IsAuthenticated) return;

        remoteGameDataService.ConfigureRemoteServices();
        remoteGameDataService.LoadCharacterSelectionData(characters =>
        {
            if (!IsAuthenticated) return;
            OnLoadCharacterFormPlayfab?.Invoke(characters);
            if (_isChangingAccount) _isChangingAccount = false;
        });
    }

    // ── Logout cleanup ────────────────────────────────────────────────────────

    private void CompleteLogout()
    {
        SaveLoadManager.Instance.SaveGame();
        ResetLocalSessionState();
        clientRuntimeService.ShutdownNetworkIfNeeded();
        clientRuntimeService.ResetClientSystems();
    }

    private void ResetLocalSessionState()
    {
        _isWaitingForGameplayConnection = false;
        authSessionService.ResetLocalSessionState();
        remoteGameDataService.ClearRemoteCache();
        clientRuntimeService.ClearBattleHistory();
    }

    // ── Flow management ───────────────────────────────────────────────────────

    private void BeginLoginFlow(bool isAutoLogin)
    {
        _currentLoginIsAuto = isAutoLogin;
        _isAutoLoginInProgress = isAutoLogin;
    }

    private void EndLoginFlow() => _isAutoLoginInProgress = false;

    // ── Đổi nhân vật ─────────────────────────────────────────────────────────

    private void CompleteCharacterSwitch()
    {
        _isWaitingForGameplayConnection = false;
        clientRuntimeService.ShutdownNetworkIfNeeded();
        clientRuntimeService.ResetClientSystems();
        gameData.ClearNotCharacterData();
        LoadCharacterDataChoose();
        NavigateToCharacterSelection();
    }

    private void NavigateToCharacterSelection()
    {
        if (navigationToCharacterSelectionScreen != null)
        {
            navigationToCharacterSelectionScreen.OnClick();
            return;
        }

        var createAccountScreen = ScreenManagerHub.Instance.Get(CreateAccountScreenName);
        createAccountScreen.NavigateTo(CreateCharacterPanelName);
    }

    private void RegisterNetworkCallbacks()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleNetcodeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleNetcodeClientDisconnected;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleNetcodeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleNetcodeClientDisconnected;
    }

    private void UnregisterNetworkCallbacks()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleNetcodeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleNetcodeClientDisconnected;
    }

    private void HandleNetcodeClientConnected(ulong clientId)
    {
        if (!_isWaitingForGameplayConnection || NetworkManager.Singleton == null)
        {
            return;
        }

        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }

        _isWaitingForGameplayConnection = false;
        SceneLoadManager.Instance.UnLoadScene("LoadingScene");
    }

    private void HandleNetcodeClientDisconnected(ulong clientId)
    {
        if (isApplicationQuitting || _isHandlingNetworkDisconnect || NetworkManager.Singleton == null)
        {
            return;
        }

        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }

        _isHandlingNetworkDisconnect = true;
        _isWaitingForGameplayConnection = false;

        SceneLoadManager.Instance.UnLoadScene("LoadingScene");
        clientRuntimeService.ShutdownNetworkIfNeeded();
        clientRuntimeService.ResetClientSystems();
        LoadCharacterDataChoose();
        NavigateToCharacterSelection();

        if (TopNotificationUI.Instance != null)
        {
            TopNotificationUI.Instance.ShowNotification("Mất kết nối với server");
        }

        _isHandlingNetworkDisconnect = false;
    }
}
