using System;
using PlayFab;

// ─────────────────────────────────────────────────────────────────────────────
// PlayfabAuthSessionService
// Điều phối toàn bộ vòng đời: đăng nhập PlayFab → tạo session → heartbeat → logout.
// Server TỰ xác định user qua auth token – client KHÔNG gửi userId.
// ─────────────────────────────────────────────────────────────────────────────
public class PlayfabAuthSessionService
{
    private readonly PlayfabSessionState _state;
    private readonly SessionApiClient _sessionApi;
    private AuthFacade _authFacade;

    public PlayfabAuthSessionService(PlayfabSessionState state)
    {
        _state      = state;
        _sessionApi = new SessionApiClient();
    }

    public AuthFacade AuthFacade      => _authFacade;
    public PlayFabClientInstanceAPI ClientApi => _state.ClientApi;
    public bool IsAuthenticated       => _state.IsAuthenticated;
    public string SessionId           => _state.SessionId;

    // ── Khởi tạo ──────────────────────────────────────────────────────────────

    public void Configure()
    {
        _state.ClientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
        _sessionApi.SetClientApi(_state.ClientApi);

        if (Configuration.Instance.startwithHost)
        {
            _authFacade = new AuthFacade(new PlayFabAuthCustomService(_state.ClientApi, true));
            _state.Ready = true;
            return;
        }

        if (Configuration.Instance.IsClientBuild())
        {
            _authFacade = new AuthFacade(new PlayFabAuthService(_state.ClientApi));
            _state.Ready = true;
        }
    }

    public bool ShouldAutoLoginClient() => Configuration.Instance.IsClientBuild();

    // ── Đăng nhập PlayFab (auth cơ bản) ───────────────────────────────────────

    public void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
        => _authFacade.Login(data, r => OnPlayFabAuthDone(r, onSuccess), onError);

    public void AutoLogin(Action<AuthResult> onSuccess, Action<AuthError> onError)
        => _authFacade.AutoLogin(r => OnPlayFabAuthDone(r, onSuccess), onError);

    public void HostLogin(Action<AuthResult> onSuccess, Action<AuthError> onError)
        => _authFacade.Login(new LoginData(), r => OnPlayFabAuthDone(r, onSuccess), onError);

    // Xóa token PlayFab phía client
    public void Logout(Action<AuthResult> onSuccess, Action<AuthError> onError)
        => _authFacade.Logout(onSuccess, onError);

    private void OnPlayFabAuthDone(AuthResult result, Action<AuthResult> onSuccess)
    {
        // Lưu PlayFabId; reset session cũ trước khi tạo mới
        _state.CurrentPlayFabId = result.userId;
        _state.HasLoggedIn      = false;
        _state.SessionId        = string.Empty;
        onSuccess?.Invoke(result);
    }

    // ── Tạo session mới sau khi auth thành công ────────────────────────────────
    // Server tạo sessionId mới, ghi đè session cũ.
    // shouldWait = true → isOnline còn true, Manager sẽ đợi 3 giây rồi retry.

    public void CreateSession(Action<SessionCreateResponse> onSuccess, Action<AuthError> onError)
    {
        _sessionApi.CreateSession(response =>
        {
            if (response == null || !response.success)
            {
                onError?.Invoke(new AuthError(
                    response != null && response.shouldWait ? "SESSION_SHOULD_WAIT" : "SESSION_CREATE_FAILED",
                    response?.message ?? "Không thể tạo phiên đăng nhập."));
                return;
            }

            _state.SessionId   = response.sessionId;
            _state.HasLoggedIn = true;
            onSuccess?.Invoke(response);
        },
        errorMsg => onError?.Invoke(new AuthError("SESSION_CREATE_FAILED", errorMsg)));
    }

    // ── Heartbeat (gọi mỗi 2 giây) ────────────────────────────────────────────
    // Client chỉ gửi sessionId; server tự tính online qua lastHeartbeat timestamp.

    public void SendHeartbeat(Action<SessionHeartbeatResponse> onSuccess, Action<AuthError> onError)
    {
        if (!_state.IsAuthenticated) return;

        _sessionApi.SendHeartbeat(_state.SessionId,
            onSuccess,
            errorMsg => onError?.Invoke(new AuthError("HEARTBEAT_FAILED", errorMsg)));
    }

    // ── Logout session (cập nhật server) ──────────────────────────────────────
    // isOnline = false, vô hiệu hóa sessionId. Nếu lỗi thì vẫn logout cục bộ.

    public void LogoutSession(Action onCompleted)
    {
        if (string.IsNullOrEmpty(_state.SessionId))
        {
            _state.MarkLoggedOut();
            onCompleted?.Invoke();
            return;
        }

        _sessionApi.LogoutSession(_state.SessionId,
            _ => { _state.MarkLoggedOut(); onCompleted?.Invoke(); },
            _ => { _state.MarkLoggedOut(); onCompleted?.Invoke(); }); // vẫn logout cục bộ khi lỗi
    }

    // ── Quản lý trạng thái cục bộ ─────────────────────────────────────────────

    public void MarkLoggedOutLocally()  => _state.MarkLoggedOut();
    public void ResetLocalSessionState() => _state.ResetSession();
}
