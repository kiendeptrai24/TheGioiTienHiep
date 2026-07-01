using System;
using PlayFab;
using PlayFab.ClientModels;

// ─────────────────────────────────────────────────────────────────────────────
// SessionApiClient
// Giao tiếp với PlayFab CloudScript cho các thao tác session.
// Server TỰ xác định user qua auth token – client KHÔNG gửi userId.
// ─────────────────────────────────────────────────────────────────────────────
public class SessionApiClient
{
    private PlayFabClientInstanceAPI _clientApi;

    public void SetClientApi(PlayFabClientInstanceAPI clientApi)
    {
        _clientApi = clientApi;
    }

    // Tạo session sau khi đăng nhập PlayFab thành công.
    // Server kiểm tra isOnline của user, nếu cần sẽ đợi 3 giây rồi tạo sessionId mới.
    public void CreateSession(Action<SessionCreateResponse> onSuccess, Action<string> onError)
    {
        Execute<SessionCreateResponse>("CreateSession",
            new { requestStartedAt = DateTime.UtcNow.ToString("o") },
            onSuccess, onError);
    }

    // Heartbeat mỗi 2 giây. Client chỉ gửi sessionId.
    // Server so sánh sessionId với DB/cache; nếu khác → shouldLogout = true.
    // Server tự tính trạng thái online qua lastHeartbeat timestamp.
    public void SendHeartbeat(string sessionId,
        Action<SessionHeartbeatResponse> onSuccess, Action<string> onError)
    {
        Execute<SessionHeartbeatResponse>("SessionHeartbeat",
            new SessionHeartbeatRequest { sessionId = sessionId },
            onSuccess, onError);
    }

    // Logout: server cập nhật isOnline = false và vô hiệu hóa sessionId.
    public void LogoutSession(string sessionId,
        Action<SessionLogoutResponse> onSuccess, Action<string> onError)
    {
        Execute<SessionLogoutResponse>("LogoutSession",
            new SessionLogoutRequest { sessionId = sessionId },
            onSuccess, onError);
    }

    private void Execute<TOut>(string function, object param,
        Action<TOut> onSuccess, Action<string> onError)
    {
        if (_clientApi == null)
        {
            onError?.Invoke("SessionApiClient: ClientApi chưa được khởi tạo.");
            return;
        }

        _clientApi.ExecuteCloudScript<TOut>(new ExecuteCloudScriptRequest
        {
            FunctionName = function,
            FunctionParameter = param,
            GeneratePlayStreamEvent = false
        },
        result =>
        {
            if (result.Error != null) { onError?.Invoke(result.Error.Message); return; }
            onSuccess?.Invoke(result.FunctionResult is TOut v ? v : default);
        },
        error => onError?.Invoke(error.ErrorMessage));
    }
}

// ─── DTOs ───────────────────────────────────────────────────────────────────
public class SessionCreateResponse
{
    public bool success;
    public string sessionId;
    public bool shouldWait;   // true → isOnline còn true, client đợi 3 giây rồi retry
    public string message;
}

[Serializable]
public class SessionHeartbeatRequest
{
    public string sessionId;
}

[Serializable]
public class SessionHeartbeatResponse
{
    public bool isValid;
    public bool shouldLogout;
    public string reason;
}

[Serializable]
public class SessionLogoutRequest
{
    public string sessionId;
}

[Serializable]
public class SessionLogoutResponse
{
    public bool success;
}
