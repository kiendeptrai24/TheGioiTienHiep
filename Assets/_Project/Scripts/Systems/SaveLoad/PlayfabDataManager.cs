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
    private bool _suppressNextDisconnectForCharacterSwitch;

    private Coroutine _heartbeatCoroutine;
    private bool _isChangingAccount;
    private bool _isAutoLoginInProgress;
    private bool _currentLoginIsAuto;
    private bool _hasLoadedCharacterSelectionData;

    public bool ready => sessionState != null && sessionState.Ready;
    public bool IsAuthenticated => authSessionService != null && authSessionService.IsAuthenticated;
    public bool IsChangingAccount => _isChangingAccount;
    public bool IsAutoLoginInProgress => _isAutoLoginInProgress;
    public bool CurrentLoginIsAuto => _currentLoginIsAuto;
    public bool HasLoadedCharacterSelectionData => _hasLoadedCharacterSelectionData;

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

        navigationToCharacterSelectionScreen = GetComponent<ActionNavigationSpecificScreen>();
        RegisterNetworkCallbacks();
    }

    protected override void Start()
    {
        base.Start();
        gameDataCenterManager = GameDataCenterManager.Instance;
        gameDataCenterManager.OnLoadGameDataCenterSuccessed += OnDataCenterReady;
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
                return;
            }

            HandleGameplayConnectionFailed("Không kết nối với server", false);
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
        if (gameDataCenterManager == null)
        {
            gameDataCenterManager = GameDataCenterManager.Instance;
        }
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
            ShutdownToLoginPage();

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

        _hasLoadedCharacterSelectionData = false;
        remoteGameDataService.ConfigureRemoteServices();
        remoteGameDataService.LoadCharacterSelectionData(characters =>
        {
            if (!IsAuthenticated) return;
            _hasLoadedCharacterSelectionData = true;
            OnLoadCharacterFormPlayfab?.Invoke(characters);
            if (_isChangingAccount) _isChangingAccount = false;
        });
    }

    // ── Logout cleanup ────────────────────────────────────────────────────────

    private void CompleteLogout()
    {
        SaveLoadManager.Instance.SaveGame();
        ResetLocalSessionState();
        ShutdownToLoginPage();
    }

    private void ResetLocalSessionState()
    {
        _isWaitingForGameplayConnection = false;
        _hasLoadedCharacterSelectionData = false;
        authSessionService.ResetLocalSessionState();
        remoteGameDataService.ClearRemoteCache();
        clientRuntimeService.ClearBattleHistory();
    }

    // ── Flow management ───────────────────────────────────────────────────────

    private void BeginLoginFlow(bool isAutoLogin)
    {
        _currentLoginIsAuto = isAutoLogin;
        _isAutoLoginInProgress = isAutoLogin;
        _hasLoadedCharacterSelectionData = false;
    }

    private void EndLoginFlow() => _isAutoLoginInProgress = false;

    // ── Đổi nhân vật ─────────────────────────────────────────────────────────

    private void CompleteCharacterSwitch()
    {
        _isWaitingForGameplayConnection = false;
        _suppressNextDisconnectForCharacterSwitch = true;
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
        NetworkManager.Singleton.OnTransportFailure -= HandleNetcodeTransportFailure;
        NetworkManager.Singleton.OnClientStopped -= HandleNetcodeClientStopped;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleNetcodeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleNetcodeClientDisconnected;
        NetworkManager.Singleton.OnTransportFailure += HandleNetcodeTransportFailure;
        NetworkManager.Singleton.OnClientStopped += HandleNetcodeClientStopped;
    }

    private void UnregisterNetworkCallbacks()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback -= HandleNetcodeClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleNetcodeClientDisconnected;
        NetworkManager.Singleton.OnTransportFailure -= HandleNetcodeTransportFailure;
        NetworkManager.Singleton.OnClientStopped -= HandleNetcodeClientStopped;
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
        if (isApplicationQuitting || _isHandlingNetworkDisconnect || _isChangingAccount || NetworkManager.Singleton == null)
        {
            return;
        }

        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }

        if (_suppressNextDisconnectForCharacterSwitch)
        {
            _suppressNextDisconnectForCharacterSwitch = false;
            return;
        }

        _isHandlingNetworkDisconnect = true;
        HandleGameplayConnectionClosed(ShouldShowServerConnectionLostMessage(NetworkManager.Singleton)
            ? "Mất kết nối với server"
            : null);
    }

    private void HandleNetcodeTransportFailure()
    {
        if (isApplicationQuitting || _isHandlingNetworkDisconnect || _suppressNextDisconnectForCharacterSwitch || _isChangingAccount)
        {
            return;
        }

        if (_isWaitingForGameplayConnection)
        {
            HandleGameplayConnectionFailed("Không thể kết nối với server", false);
            return;
        }

        HandleGameplayConnectionClosed("Mất kết nối với server");
    }

    private void HandleNetcodeClientStopped(bool isHostMode)
    {
        if (isHostMode || isApplicationQuitting || _isHandlingNetworkDisconnect || _suppressNextDisconnectForCharacterSwitch || _isChangingAccount)
        {
            return;
        }

        if (_isWaitingForGameplayConnection)
        {
            HandleGameplayConnectionFailed("Khong ket noi duoc toi server", false);
            return;
        }

        HandleGameplayConnectionClosed("Mất kết nối với server");
    }

    private static bool ShouldShowServerConnectionLostMessage(NetworkManager networkManager)
    {
        if (networkManager == null)
        {
            return false;
        }

        string disconnectReason = networkManager.DisconnectReason;
        if (!string.IsNullOrEmpty(disconnectReason))
        {
            return disconnectReason.IndexOf("server shutting down", StringComparison.OrdinalIgnoreCase) >= 0
                   || disconnectReason.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0
                   || disconnectReason.IndexOf("timed out", StringComparison.OrdinalIgnoreCase) >= 0
                   || disconnectReason.IndexOf("closed by remote", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        return networkManager.DisconnectEvent == NetworkTransport.DisconnectEvents.ProtocolTimeout
               || networkManager.DisconnectEvent == NetworkTransport.DisconnectEvents.ClosedByRemote
               || networkManager.DisconnectEvent == NetworkTransport.DisconnectEvents.ProtocolError
               || networkManager.DisconnectEvent == NetworkTransport.DisconnectEvents.MaxConnectionAttempts;
    }

    private void ShutdownToLoginPage()
    {
        var networkManager = NetworkManager.Singleton;
        bool hasActiveSession = networkManager != null &&
                                (networkManager.IsListening || networkManager.ShutdownInProgress);

        clientRuntimeService.ShutdownNetworkIfNeeded();

        if (!hasActiveSession)
        {
            clientRuntimeService.ResetClientSystems();
        }
    }

    private void HandleGameplayConnectionFailed(string notificationMessage, bool requiresPendingConnection = true)
    {
        if (requiresPendingConnection && !_isWaitingForGameplayConnection)
        {
            return;
        }

        _isHandlingNetworkDisconnect = true;
        HandleGameplayConnectionClosed(notificationMessage);
    }

    private void HandleGameplayConnectionClosed(string notificationMessage)
    {
        _isWaitingForGameplayConnection = false;

        if (!string.IsNullOrEmpty(notificationMessage) && TopNotificationUI.Instance != null)
        {
            TopNotificationUI.Instance.ShowNotification(notificationMessage);
        }

        SceneLoadManager.Instance.UnLoadScene("LoadingScene");
        clientRuntimeService.ShutdownNetworkIfNeeded();
        clientRuntimeService.ResetClientSystems();

        _isHandlingNetworkDisconnect = false;
    }
}
