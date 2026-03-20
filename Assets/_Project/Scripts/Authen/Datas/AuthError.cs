using System;

[Serializable]
public class AuthError
{
    public string code;
    public string message;

    public AuthError(string code, string message)
    {
        this.code = code;
        this.message = message;
    }
}