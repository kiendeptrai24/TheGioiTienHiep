using System;

public interface IAuthService
{
    void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError);
    void Register(RegisterData data, Action<AuthResult> onSuccess, Action<AuthError> onError);
    void ForgotPassword(ForgotPasswordData data, Action<string> onSuccess, Action<AuthError> onError);
}