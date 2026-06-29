using System;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabRealtimeSessionService
{
    private readonly PlayFabClientInstanceAPI clientApi;

    public PlayFabRealtimeSessionService(PlayFabClientInstanceAPI clientApi)
    {
        this.clientApi = clientApi;
    }

    public void TryAcquireLock(string playFabId, string sessionId, string requestStartedAt, Action<CloudSessionRequestResult> onResult, Action<AuthError> onError)
    {
        ExecuteCloudScript<CloudSessionRequestResult>("RequestSession", new
        {
            playFabId,
            sessionId,
            requestStartedAt
        }, result =>
        {
            if (result == null)
            {
                onError?.Invoke(new AuthError("PLAYFAB_SESSION_REQUEST_FAILED", "Khong the tao session online."));
                return;
            }

            onResult?.Invoke(result);
        }, onError);
    }

    public void RefreshLock(string playFabId, string sessionId, Action<CloudSessionHeartbeatResult> onSuccess, Action<AuthError> onError)
    {
        ExecuteCloudScript<CloudSessionHeartbeatResult>("Heartbeat", new
        {
            playFabId,
            sessionId
        }, result =>
        {
            onSuccess?.Invoke(result);
        }, onError);
    }

    public void ReleaseLock(string sessionId, Action onSuccess, Action<AuthError> onError)
    {
        ExecuteCloudScript<CloudSessionReleaseResult>("ReleaseSession", new
        {
            sessionId
        }, _ => { onSuccess?.Invoke(); }, onError);
    }

    private void ExecuteCloudScript<TOut>(string functionName, object functionParameter, Action<TOut> onSuccess, Action<AuthError> onError)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = functionName,
            FunctionParameter = functionParameter,
            GeneratePlayStreamEvent = false
        };

        clientApi.ExecuteCloudScript<TOut>(request, result =>
        {
            if (result.Error != null)
            {
                onError?.Invoke(new AuthError("PLAYFAB_CLOUDSCRIPT_ERROR", result.Error.Message));
                return;
            }

            if (result.FunctionResult is TOut payload)
            {
                onSuccess?.Invoke(payload);
                return;
            }

            onSuccess?.Invoke(default);
        }, error =>
        {
            onError?.Invoke(new AuthError("PLAYFAB_CLOUDSCRIPT_REQUEST_FAILED", error.ErrorMessage));
        });
    }
}

[Serializable]
public class CloudSessionRequestResult
{
    public bool success;
    public string status;
    public bool kickedPreviousSession;
    public string previousSessionId;
    public string activeSessionId;
    public string message;
    public string errorCode;
}

[Serializable]
public class CloudSessionHeartbeatResult
{
    public bool valid;
    public bool shouldLogout;
    public string reason;
}

[Serializable]
public class CloudSessionReleaseResult
{
    public bool released;
    public bool pendingActivated;
}
