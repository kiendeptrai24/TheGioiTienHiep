using System;

public class AuthFacade
{
    private readonly IAuthService authService;

    public AuthFacade(IAuthService authService)
    {
        this.authService = authService;
    }

    public void AutoLogin(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authService.AutoLogin(onSuccess, onError);
    }

    public void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authService.Login(data, onSuccess, onError);
    }
    public void Logout(Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authService.Logout(onSuccess, onError);
    }
    public void Register(RegisterData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authService.Register(data, onSuccess, onError);
    }

    public void ForgotPassword(ForgotPasswordData data, Action<string> onSuccess, Action<AuthError> onError)
    {
        authService.ForgotPassword(data, onSuccess, onError);
    }
}