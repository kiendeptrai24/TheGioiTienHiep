using System;
using PlayFab;

public class PlayfabAuthSessionService
{
    private readonly PlayfabSessionState state;

    private AuthFacade authFacade;
    private PlayFabRealtimeSessionService realtimeSessionService;

    public PlayfabAuthSessionService(PlayfabSessionState state)
    {
        this.state = state;
    }

    public AuthFacade AuthFacade => authFacade;
    public PlayFabClientInstanceAPI ClientApi => state.ClientApi;
    public bool Ready => state.Ready;
    public bool IsAuthenticated => state.IsAuthenticated;
    public bool HasSessionLock => state.SessionLockAcquired;
    public string SessionId => state.SessionId;
    public string CurrentPlayFabId => state.CurrentPlayFabId;

    public void Configure()
    {
        state.ClientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
        realtimeSessionService = new PlayFabRealtimeSessionService(state.ClientApi);

        if (Configuration.Instance.startwithHost)
        {
            authFacade = new AuthFacade(new PlayFabAuthCustomService(state.ClientApi, true));
            state.Ready = true;
            return;
        }

        if (Configuration.Instance.IsClientBuild())
        {
            authFacade = new AuthFacade(new PlayFabAuthService(state.ClientApi));
            state.Ready = true;
        }
    }

    public bool ShouldAutoLoginClient()
    {
        return Configuration.Instance.IsClientBuild();
    }

    public bool ShouldUseRealtimeSession()
    {
        return Configuration.Instance.IsClientBuild() || Configuration.Instance.startwithHost;
    }

    public void Login(LoginData loginData, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authFacade.Login(loginData, result => HandleLoginSuccess(result, onSuccess), onError);
    }

    public void AutoLogin(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authFacade.AutoLogin(result => HandleLoginSuccess(result, onSuccess), onError);
    }

    public void HostLogin(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authFacade.Login(new LoginData(), result => HandleLoginSuccess(result, onSuccess), onError);
    }

    public void Logout(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authFacade.Logout(onSuccess, onError);
    }

    public void PrepareAuthenticatedSession(AuthResult result)
    {
        state.SessionId = string.IsNullOrEmpty(result.sessionId) ? Guid.NewGuid().ToString() : result.sessionId;
        result.sessionId = state.SessionId;
        state.CurrentPlayFabId = result.userId;
    }

    public void AcquireRealtimeSession(AuthResult result, Action<AuthResult> onSuccess, Action<AuthError> onError, Action<CloudSessionRequestResult> onWaiting = null)
    {
        if (!ShouldUseRealtimeSession())
        {
            onSuccess?.Invoke(result);
            return;
        }

        PrepareAuthenticatedSession(result);
        realtimeSessionService.TryAcquireLock(state.CurrentPlayFabId, state.SessionId, sessionResult =>
        {
            var isActive = sessionResult.success &&
                (sessionResult.status == "ACTIVE" || string.IsNullOrEmpty(sessionResult.status));

            if (isActive)
            {
                state.SessionLockAcquired = true;
                state.HasLoggedIn = true;
                onSuccess?.Invoke(result);
                return;
            }

            if (!sessionResult.success && sessionResult.status == "WAITING")
            {
                onWaiting?.Invoke(sessionResult);
                return;
            }

            if (!sessionResult.success && (sessionResult.status == "TIMEOUT" || sessionResult.status == "ERROR"))
            {
                onError?.Invoke(new AuthError(
                    string.IsNullOrEmpty(sessionResult.errorCode) ? "PLAYFAB_SESSION_REQUEST_FAILED" : sessionResult.errorCode,
                    string.IsNullOrEmpty(sessionResult.message) ? "Khong the tao session online." : sessionResult.message));
                return;
            }

            onError?.Invoke(new AuthError("PLAYFAB_SESSION_REQUEST_FAILED", "Khong the tao session online."));
        }, onError);
    }

    public void RefreshRealtimeSessionLock(Action<CloudSessionHeartbeatResult> onSuccess, Action<AuthError> onError)
    {
        if (!state.SessionLockAcquired ||
            string.IsNullOrEmpty(state.CurrentPlayFabId) ||
            string.IsNullOrEmpty(state.SessionId))
        {
            return;
        }

        realtimeSessionService.RefreshLock(state.CurrentPlayFabId, state.SessionId, onSuccess, onError);
    }

    public void ReleaseRealtimeSessionLock(Action onReleased, Action<AuthError> onError)
    {
        if (!state.SessionLockAcquired || string.IsNullOrEmpty(state.SessionId))
        {
            onReleased?.Invoke();
            return;
        }

        realtimeSessionService.ReleaseLock(state.SessionId, () =>
        {
            state.SessionLockAcquired = false;
            onReleased?.Invoke();
        }, error =>
        {
            state.SessionLockAcquired = false;
            onError?.Invoke(error);
            onReleased?.Invoke();
        });
    }

    public void RetryAcquireSession(AuthResult authResult, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        if (!ShouldUseRealtimeSession() || string.IsNullOrEmpty(state.SessionId))
        {
            onSuccess?.Invoke(authResult);
            return;
        }

        realtimeSessionService.TryAcquireLock(state.CurrentPlayFabId, state.SessionId, sessionResult =>
        {
            var isActive = sessionResult.success &&
                (sessionResult.status == "ACTIVE" || string.IsNullOrEmpty(sessionResult.status));

            if (isActive)
            {
                state.SessionLockAcquired = true;
                state.HasLoggedIn = true;
                onSuccess?.Invoke(authResult);
                return;
            }

            if (!sessionResult.success && sessionResult.status == "WAITING")
            {
                onError?.Invoke(new AuthError("SESSION_STILL_WAITING", string.IsNullOrEmpty(sessionResult.message) ? "Phien cuoc truoc van dang hoat dong." : sessionResult.message));
                return;
            }

            if (!sessionResult.success && (sessionResult.status == "TIMEOUT" || sessionResult.status == "ERROR"))
            {
                onError?.Invoke(new AuthError(
                    string.IsNullOrEmpty(sessionResult.errorCode) ? "PLAYFAB_SESSION_REQUEST_FAILED" : sessionResult.errorCode,
                    string.IsNullOrEmpty(sessionResult.message) ? "Khong the tao session online." : sessionResult.message));
                return;
            }

            onError?.Invoke(new AuthError("PLAYFAB_SESSION_REQUEST_FAILED", "Khong the tao session online."));
        }, onError);
    }

    public void MarkLoggedOutLocally()
    {
        state.MarkLoggedOut();
    }

    public void ResetLocalSessionState()
    {
        state.ResetSession();
    }

    private void HandleLoginSuccess(AuthResult result, Action<AuthResult> onSuccess)
    {
        state.HasLoggedIn = false;
        state.SessionLockAcquired = false;
        onSuccess?.Invoke(result);
    }
}
