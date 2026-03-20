using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabAuthCustomService : AuthServiceBase
{
    public override void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {

        string playerLoginId = "testLogin1";
        var clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);

        var request = new LoginWithCustomIDRequest { CustomId = playerLoginId, CreateAccount = true };

        clientApi.LoginWithCustomID(request,
        onResSuccess =>
        {
            onSuccess?.Invoke(new AuthResult
            {
                clientApi = clientApi,
                userId = onResSuccess.PlayFabId,
                email = data.email,
                accessToken = "playfab_token",
                message = "Đăng nhập thành công"
            });
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
}