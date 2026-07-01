using System.Collections.Generic;
using PlayFab;

// Lưu trạng thái session hiện tại của một người dùng đã đăng nhập.
public class PlayfabSessionState
{
    public PlayfabSessionState(GameData gameData) => GameData = gameData;

    public GameData GameData { get; }
    public List<ILoadRemote<GameData>> LoadRemotes { get; } = new();
    public List<ISaveRemote<GameData>> SaveRemotes { get; } = new();

    public bool Ready { get; set; }
    public bool HasLoggedIn { get; set; }
    public string SessionId { get; set; } = string.Empty;  // do server cấp
    public string CurrentPlayFabId { get; set; } = string.Empty;
    public PlayFabClientInstanceAPI ClientApi { get; set; }

    // Đã xác thực đầy đủ: có login + sessionId + playFabId
    public bool IsAuthenticated =>
        HasLoggedIn &&
        !string.IsNullOrEmpty(SessionId) &&
        !string.IsNullOrEmpty(CurrentPlayFabId);

    public void MarkLoggedOut()
    {
        HasLoggedIn = false;
    }

    public void ResetSession()
    {
        HasLoggedIn = false;
        SessionId = string.Empty;
        CurrentPlayFabId = string.Empty;
        LoadRemotes.Clear();
        SaveRemotes.Clear();
    }
}
