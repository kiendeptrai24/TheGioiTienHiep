using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
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

    private Coroutine _heartbeatCoroutine;
    private Coroutine _sessionRetryCoroutine;
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
        CancelSessionRetry();
        authSessionService.MarkLoggedOutLocally();
    }

    protected override void OnDestroy()
    {
        StopHeartbeat();
        CancelSessionRetry();

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
        authSessionService.CreateSession(OnSessionCreateSuccess, OnSessionCreateError);
    }

    private void OnSessionCreateSuccess(SessionCreateResponse response)
    {
        LoginStatusChanged?.Invoke("Đăng nhập thành công.");
        StartHeartbeat();

        var authResult = new AuthResult
        {
            userId = sessionState.CurrentPlayFabId,
            sessionId = sessionState.SessionId
        };
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
        if (error.code == "SESSION_SHOULD_WAIT")
        {
            // isOnline còn true → đợi 3 giây rồi retry
            LoginStatusChanged?.Invoke(
                string.IsNullOrEmpty(error.message)
                    ? "Tài khoản đang online ở thiết bị khác. Đang chờ..."
                    : error.message);

            CancelSessionRetry();
            _sessionRetryCoroutine = StartCoroutine(SessionShouldWaitRetryRoutine());
            return;
        }

        ResetLocalSessionState();
        LoginError?.Invoke(error);
    }

    private IEnumerator SessionShouldWaitRetryRoutine()
    {
        yield return new WaitForSeconds(SessionWaitBeforeRetrySeconds);

        if (isApplicationQuitting) yield break;

        while (!isApplicationQuitting)
        {
            Debug.Log("[PlayfabDataManager] Heartbeat phiên cũ còn mới, chờ 3 giây rồi thử login session lại.");

            bool completed = false;
            bool shouldRetry = false;
            AuthError fatalError = null;

            authSessionService.CreateSession(
                response =>
                {
                    completed = true;
                    _sessionRetryCoroutine = null;
                    OnSessionCreateSuccess(response);
                },
                error =>
                {
                    completed = true;
                    shouldRetry = error.code == "SESSION_SHOULD_WAIT";
                    if (!shouldRetry)
                    {
                        fatalError = error;
                    }
                });

            yield return new WaitUntil(() => completed);

            if (fatalError == null && !shouldRetry)
            {
                _sessionRetryCoroutine = null;
                yield break;
            }

            if (fatalError != null)
            {
                _sessionRetryCoroutine = null;
                ResetLocalSessionState();
                LoginError?.Invoke(fatalError);
                yield break;
            }

            yield return new WaitForSeconds(SessionWaitBeforeRetrySeconds);
        }

        _sessionRetryCoroutine = null;
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
        authSessionService.ResetLocalSessionState();
        remoteGameDataService.ClearRemoteCache();
        clientRuntimeService.ClearBattleHistory();
    }

    // ── Flow management ───────────────────────────────────────────────────────

    private void CancelSessionRetry()
    {
        if (_sessionRetryCoroutine != null)
        {
            StopCoroutine(_sessionRetryCoroutine);
            _sessionRetryCoroutine = null;
        }
        EndLoginFlow();
    }

    private void BeginLoginFlow(bool isAutoLogin)
    {
        _currentLoginIsAuto = isAutoLogin;
        _isAutoLoginInProgress = isAutoLogin;
    }

    private void EndLoginFlow() => _isAutoLoginInProgress = false;

    // ── Đổi nhân vật ─────────────────────────────────────────────────────────

    private void CompleteCharacterSwitch()
    {
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
}
