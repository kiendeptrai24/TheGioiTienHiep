using System.Collections.Generic;
using PlayFab;

public class PlayfabSessionState
{
    public PlayfabSessionState(GameData gameData)
    {
        GameData = gameData;
    }

    public GameData GameData { get; }
    public List<ILoadRemote<GameData>> LoadRemotes { get; } = new();
    public List<ISaveRemote<GameData>> SaveRemotes { get; } = new();

    public bool Ready { get; set; }
    public bool HasLoggedIn { get; set; }
    public bool SessionLockAcquired { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string CurrentPlayFabId { get; set; } = string.Empty;
    public PlayFabClientInstanceAPI ClientApi { get; set; }

    public bool IsAuthenticated =>
        HasLoggedIn &&
        SessionLockAcquired &&
        !string.IsNullOrEmpty(CurrentPlayFabId);

    public void MarkLoggedOut()
    {
        HasLoggedIn = false;
    }

    public void ResetSession()
    {
        HasLoggedIn = false;
        SessionLockAcquired = false;
        SessionId = string.Empty;
        CurrentPlayFabId = string.Empty;
        LoadRemotes.Clear();
        SaveRemotes.Clear();
    }
}
