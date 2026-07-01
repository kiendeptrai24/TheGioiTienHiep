using System;
using PlayFab;

[Serializable]
public class AuthResult
{
    public PlayFabClientInstanceAPI clientApi;
    public string userId;
    public string displayName;
    public string email;
    public string accessToken;
    public string sessionId;
    public string message;
    public bool shouldWaitBeforeEnter;
    public float waitBeforeEnterSeconds;
    internal string ipAddress;
    internal int port;
}
