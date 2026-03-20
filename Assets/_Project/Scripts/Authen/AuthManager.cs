using System;

public class AuthManager
{
    private readonly IAuthService authService;

    public AuthManager(IAuthService authService)
    {
        this.authService = authService;
    }

    public void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        authService.Login(data, onSuccess, onError);
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