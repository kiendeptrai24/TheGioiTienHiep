using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabAuthCustomService : AuthServiceBase
{
    private PlayFabClientInstanceAPI clientAPI;
    private bool isServer = false;
    public PlayFabAuthCustomService(PlayFabClientInstanceAPI clientAPI, bool isServer = false)
    {
        this.clientAPI = clientAPI;
        this.isServer = isServer;
    }
    private const string CUSTOM_ID_KEY = "CUSTOM_ID";
    public override void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        string playerLoginId = isServer ? "Server" : Guid.NewGuid().ToString();

        var request = new LoginWithCustomIDRequest { CustomId = playerLoginId, CreateAccount = true };
        clientAPI.LoginWithCustomID(request,
        onResSuccess =>
        {
            onSuccess?.Invoke(new AuthResult
            {
                clientApi = clientAPI,
                userId = onResSuccess.PlayFabId,
                email = data.email,
                accessToken = onResSuccess.EntityToken.EntityToken,
                sessionId = Guid.NewGuid().ToString(),
                message = "Đăng nhập thành công"
            });
            PlayerPrefs.SetString(CUSTOM_ID_KEY, playerLoginId);
            PlayerPrefs.Save();
            Debug.Log("Login call succeeded.");
        }, onResError =>
        {
            onError?.Invoke(new AuthError("PLAYFAB_LOGIN_FAILED", onResError.GenerateErrorReport()));
        });
    }

    public override void Register(RegisterData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {

    }

    public override void ForgotPassword(ForgotPasswordData data, Action<string> onSuccess, Action<AuthError> onError)
    {

    }

    public override void Logout(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        PlayerPrefs.DeleteKey(CUSTOM_ID_KEY);
        PlayerPrefs.Save();
        onSuccess?.Invoke(new AuthResult
        {
            message = "Đăng xuất thành công"
        });
    }

    public override void AutoLogin(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        if (!PlayerPrefs.HasKey(CUSTOM_ID_KEY))
        {
            Debug.Log("Chưa có thông tin login");
            return;
        }

        string custemId = PlayerPrefs.GetString(CUSTOM_ID_KEY);

        var request = new LoginWithCustomIDRequest { CustomId = custemId, CreateAccount = true };

        // Demo giả lập

        clientAPI.LoginWithCustomID(request,
        onResSuccess =>
        {
            onSuccess?.Invoke(new AuthResult
            {
                clientApi = clientAPI,
                userId = onResSuccess.PlayFabId,
                accessToken = onResSuccess.SessionTicket,
                message = "Đăng nhập thành công"
            });
        }
        , onResError =>
        {
            onError?.Invoke(new AuthError("PLAYFAB_LOGIN_FAILED", onResError.GenerateErrorReport()));
        });
    }
}