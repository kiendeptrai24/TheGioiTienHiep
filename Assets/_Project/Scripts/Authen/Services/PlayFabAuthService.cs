using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabAuthService : AuthServiceBase
{
    public PlayFabClientInstanceAPI clientAPI;
    public PlayFabAuthService(PlayFabClientInstanceAPI clientAPI)
    {
        this.clientAPI = clientAPI;
    }
    private const string EMAIL_KEY = "EMAIL";
    private const string PASSWORD_KEY = "PASSWORD";
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
        clientAPI.LoginWithEmailAddress(request,
        onResSuccess =>
        {
            onSuccess?.Invoke(new AuthResult
            {
                clientApi = clientAPI,
                userId = onResSuccess.PlayFabId,
                email = data.email,
                accessToken = onResSuccess.SessionTicket,
                sessionId = Guid.NewGuid().ToString(),
                message = "Đăng nhập thành công"
            });
            PlayerPrefs.SetString(EMAIL_KEY, data.email);
            PlayerPrefs.SetString(PASSWORD_KEY, data.password);
            PlayerPrefs.Save();
        }
        , onResError =>
        {
            HandlePlayFabError(onResError, onError);
        });

    }
    private void HandlePlayFabError(PlayFabError error, Action<AuthError> onError)
    {
        string code = error.Error.ToString();
        string message = GetPlayFabErrorMessage(error);

        Debug.LogWarning(
            $"PlayFab Error\n" +
            $"Code: {code}\n" +
            $"HttpCode: {error.HttpCode}\n" +
            $"HttpStatus: {error.HttpStatus}\n" +
            $"Message: {error.ErrorMessage}\n" +
            $"Report: {error.GenerateErrorReport()}"
        );

        onError?.Invoke(new AuthError(code, message));
    }
    private string GetPlayFabErrorMessage(PlayFabError error)
    {
        switch (error.Error)
        {
            case PlayFabErrorCode.AccountNotFound:
                return "Tài khoản không tồn tại.";

            case PlayFabErrorCode.InvalidUsernameOrPassword:
            case PlayFabErrorCode.InvalidEmailOrPassword:
                return "Sai email hoặc mật khẩu.";

            case PlayFabErrorCode.InvalidEmailAddress:
                return "Email không hợp lệ.";

            case PlayFabErrorCode.EmailAddressNotAvailable:
            case PlayFabErrorCode.DuplicateEmail:
                return "Email này đã được đăng ký.";

            case PlayFabErrorCode.InvalidPassword:
                return "Mật khẩu không hợp lệ.";

            case PlayFabErrorCode.UsernameNotAvailable:
            case PlayFabErrorCode.DuplicateUsername:
                return "Tên tài khoản đã tồn tại.";

            case PlayFabErrorCode.AccountBanned:
                return "Tài khoản đã bị khóa.";

            case PlayFabErrorCode.InvalidParams:
                return "Thông tin nhập vào không hợp lệ.";

            case PlayFabErrorCode.ConnectionError:
                return "Không thể kết nối tới PlayFab. Kiểm tra mạng.";

            case PlayFabErrorCode.ServiceUnavailable:
            case PlayFabErrorCode.DownstreamServiceUnavailable:
                return "Máy chủ PlayFab đang bận. Vui lòng thử lại sau.";

            case PlayFabErrorCode.APIClientRequestRateLimitExceeded:
            case PlayFabErrorCode.APIRequestLimitExceeded:
            case PlayFabErrorCode.OverLimit:
                return "Bạn thao tác quá nhanh. Vui lòng thử lại sau.";

            case PlayFabErrorCode.NotAuthorized:
            case PlayFabErrorCode.NotAuthenticated:
            case PlayFabErrorCode.InvalidSessionTicket:
                return "Phiên đăng nhập không hợp lệ.";

            case PlayFabErrorCode.InvalidTitleId:
                return "PlayFab TitleId không đúng.";

            default:
                return $"Lỗi PlayFab: {error.ErrorMessage}";
        }
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
            HandlePlayFabError(onResError, onError);
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

    public override void Logout(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        PlayerPrefs.DeleteKey(EMAIL_KEY);
        PlayerPrefs.DeleteKey(PASSWORD_KEY);
        PlayerPrefs.Save();
        onSuccess?.Invoke(new AuthResult
        {
            message = "Đăng xuất thành công"
        });
    }

    public override void AutoLogin(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        if (!PlayerPrefs.HasKey(EMAIL_KEY) || !PlayerPrefs.HasKey(PASSWORD_KEY))
        {
            Debug.Log("Chưa có thông tin login");
            return;
        }

        string email = PlayerPrefs.GetString(EMAIL_KEY);
        string password = PlayerPrefs.GetString(PASSWORD_KEY);

        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };


        clientAPI.LoginWithEmailAddress(request,
        onResSuccess =>
        {
            onSuccess?.Invoke(new AuthResult
            {
                clientApi = clientAPI,
                userId = onResSuccess.PlayFabId,
                email = email,
                accessToken = onResSuccess.SessionTicket,
                message = "Đăng nhập thành công"
            });
        }
        , onResError =>
        {
            HandlePlayFabError(onResError, onError);
        });
    }
}