using System;

public abstract class AuthServiceBase : IAuthService
{
    public abstract void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError);
    public abstract void AutoLogin(Action<AuthResult> onSuccess, Action<AuthError> onError);
    public abstract void Register(RegisterData data, Action<AuthResult> onSuccess, Action<AuthError> onError);
    public abstract void ForgotPassword(ForgotPasswordData data, Action<string> onSuccess, Action<AuthError> onError);
    public abstract void Logout(Action<AuthResult> onSuccess, Action<AuthError> onError);

    protected bool IsNullOrEmpty(string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    protected bool ValidateLogin(LoginData data, Action<AuthError> onError)
    {
        if (data == null)
        {
            onError?.Invoke(new AuthError("LOGIN_DATA_NULL", "Login data is null"));
            return false;
        }

        if (IsNullOrEmpty(data.password))
        {
            onError?.Invoke(new AuthError("LOGIN_PASSWORD_EMPTY", "Password không được để trống"));
            return false;
        }

        return true;
    }

    protected bool ValidateRegister(RegisterData data, Action<AuthError> onError)
    {
        if (data == null)
        {
            onError?.Invoke(new AuthError("REGISTER_DATA_NULL", "Register data is null"));
            return false;
        }

        if (IsNullOrEmpty(data.email))
        {
            onError?.Invoke(new AuthError("REGISTER_EMAIL_EMPTY", "Email không được để trống"));
            return false;
        }

        if (IsNullOrEmpty(data.password))
        {
            onError?.Invoke(new AuthError("REGISTER_PASSWORD_EMPTY", "Password không được để trống"));
            return false;
        }

        if (data.password != data.confirmPassword)
        {
            onError?.Invoke(new AuthError("REGISTER_PASSWORD_NOT_MATCH", "Confirm password không khớp"));
            return false;
        }

        return true;
    }

    protected bool ValidateForgotPassword(ForgotPasswordData data, Action<AuthError> onError)
    {
        if (data == null)
        {
            onError?.Invoke(new AuthError("FORGOT_DATA_NULL", "Forgot password data is null"));
            return false;
        }

        if (IsNullOrEmpty(data.email))
        {
            onError?.Invoke(new AuthError("FORGOT_EMAIL_EMPTY", "Email không được để trống"));
            return false;
        }

        return true;
    }
}