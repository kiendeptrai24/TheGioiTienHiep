using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabAuthService : AuthServiceBase
{
    public override void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        if (!ValidateLogin(data, onError))
            return;
        var request = new LoginWithEmailAddressRequest()
        {
            Email = data.email,
            Password = data.password,
        };
        // Demo giả lập
        var clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);

        clientApi.LoginWithEmailAddress(request,
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
        }
        , onResError =>
        {

            onError?.Invoke(new AuthError("PLAYFAB_LOGIN_FAILED", onResError.GenerateErrorReport()));
        });

    }

    public override void Register(RegisterData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        if (!ValidateRegister(data, onError))
            return;

        Debug.Log("PlayFab Register...");

        var request = new RegisterPlayFabUserRequest()
        {
            Email = data.email,
            Password = data.password,
            RequireBothUsernameAndEmail = false,
        };
        // Demo giả lập
        PlayFabClientAPI.RegisterPlayFabUser(request,
        onResSuccess =>
        {
            onSuccess?.Invoke(new AuthResult
            {

                userId = "PF_NEW_001",
                email = data.email,
                accessToken = "playfab_register_token",
                message = "Đăng ký thành công"
            });

        }
        , onResError =>
        {
            onError?.Invoke(new AuthError("PLAYFAB_REGISTER_FAILED", onResError.GenerateErrorReport()));
        });
    }

    public override void ForgotPassword(ForgotPasswordData data, Action<string> onSuccess, Action<AuthError> onError)
    {
        if (!ValidateForgotPassword(data, onError))
            return;

        var request = new SendAccountRecoveryEmailRequest
        {
            Email = data.email,
            TitleId = data.titleId,
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request,
        onResSuccess =>
        {
            onSuccess?.Invoke("Đã gửi email đặt lại mật khẩu");
        }
        , onResError =>
        {
            onError?.Invoke(new AuthError("PLAYFAB_FORGOT_FAILED", onResError.GenerateErrorReport()));
        });
    }
}