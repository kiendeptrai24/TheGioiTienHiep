using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class PlayFabAuthServiceRemote : AuthServiceBase
{
    public PlayFabClientInstanceAPI clientAPI;
    public Configuration configuration;
    public PlayFabAuthServiceRemote(PlayFabClientInstanceAPI clientAPI)
    {
        this.clientAPI = clientAPI;
        configuration = Configuration.Instance;
    }
    private const string EMAIL_KEY = "EMAIL";
    private const string PASSWORD_KEY = "PASSWORD";
    public override void Login(LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        if (!ValidateLogin(data, onError))
            return;

        var request = new LoginWithEmailAddressRequest
        {
            TitleId = PlayFabSettings.TitleId,
            Email = data.email,
            Password = data.password,
        };

        clientAPI.LoginWithEmailAddress(
            request,
            onResSuccess =>
            {
                PlayFabAuthenticationAPI.GetEntityToken(
                    new PlayFab.AuthenticationModels.GetEntityTokenRequest(),
                    entityResult =>
                    {
                        bool hasServerInfo =
                            !string.IsNullOrEmpty(configuration.ipAddress) &&
                            configuration.port > 0;

                        if (hasServerInfo)
                        {
                            Debug.Log($"Dùng server có sẵn: {configuration.ipAddress}:{configuration.port}");
                            OnPlayFabLoginSuccess(onResSuccess, data, onSuccess, onError);
                            return;
                        }

                        var serverRequest = new PlayFab.MultiplayerModels.RequestMultiplayerServerRequest
                        {
                            BuildId = configuration.buildId,
                            SessionId = Guid.NewGuid().ToString(),
                            PreferredRegions = new List<string> { "EastUs" }
                        };

                        PlayFabMultiplayerAPI.RequestMultiplayerServer(
                            serverRequest,
                            serverResult =>
                            {
                                configuration.ipAddress = serverResult.IPV4Address;
                                configuration.port = (ushort)serverResult.Ports[0].Num;

                                Debug.Log($"Request server thành công: {configuration.ipAddress}:{configuration.port}");
                                OnPlayFabLoginSuccess(onResSuccess, data, onSuccess, onError);
                            },
                            error =>
                            {
                                onError?.Invoke(new AuthError(
                                    "PLAYFAB_REQUEST_SERVER_FAILED",
                                    error.GenerateErrorReport()));
                            });
                    },
                    entityError =>
                    {
                        onError?.Invoke(new AuthError(
                            "PLAYFAB_ENTITY_TOKEN_FAILED",
                            entityError.GenerateErrorReport()
                        ));
                    });
            },
            onResError =>
            {
                onError?.Invoke(new AuthError(
                    "PLAYFAB_LOGIN_FAILED",
                    onResError.GenerateErrorReport()
                ));
            });

    }
    private void OnPlayFabLoginSuccess(LoginResult response, LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        Debug.Log("[ClientStartUp].OnPlayFabLoginSuccess: " + response.PlayFabId);

        if (string.IsNullOrEmpty(configuration.ipAddress))
        {
            //We need to grab an IP and Port from a server based on the buildId. Copy this and add it to your Configuration.
            RequestMultiplayerServer(response, data, onSuccess, onError);
        }
        else
        {
            ConnectRemoteClient(null, response, data, onSuccess);
        }
    }
    private void RequestMultiplayerServer(LoginResult response, LoginData data, Action<AuthResult> onSuccess, Action<AuthError> onError)
    {
        Debug.Log("[ClientStartUp].RequestMultiplayerServer");

        var requestData = new RequestMultiplayerServerRequest
        {
            BuildId = configuration.buildId,
            SessionId = Guid.NewGuid().ToString(),
            PreferredRegions = new List<string> { AzureRegion.EastUs.ToString() }
        };

        PlayFabMultiplayerAPI.RequestMultiplayerServer(
            requestData,
            resultCallback =>
            {
                OnRequestMultiplayerServer(resultCallback, response, data, onSuccess);
            },
            errorCallback =>
            {
                OnRequestMultiplayerServerError(errorCallback, onError);
            });
    }
    private void OnRequestMultiplayerServerError(PlayFabError error, Action<AuthError> onError)
    {
        Debug.LogWarning(error.GenerateErrorReport());
        onError?.Invoke(new AuthError("PLAYFAB_LOGIN_FAILED", error.GenerateErrorReport()));
    }
    private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response, LoginResult loginResult, LoginData data, Action<AuthResult> onSuccess)
    {
        Debug.Log("[ClientStartUp].OnRequestMultiplayerServer: " + response.ToString());
        ConnectRemoteClient(response, loginResult, data, onSuccess);
    }

    private void ConnectRemoteClient(RequestMultiplayerServerResponse response = null, LoginResult loginResult = null, LoginData data = null, Action<AuthResult> onSuccess = null)
    {
        Debug.Log("[ClientStartUp].ConnectRemoteClient (NGO)");

        string ip;
        ushort port;

        if (response == null)
        {
            // Dùng config có sẵn
            ip = configuration.ipAddress;
            port = configuration.port;
        }
        else
        {
            // Lấy IP / Port từ PlayFab
            ip = response.IPV4Address;
            port = (ushort)response.Ports[0].Num;

            Debug.Log($"**** ADD THIS TO YOUR CONFIGURATION **** -- IP: {ip} Port: {port}");
        }

        var utp = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        if (utp == null)
        {
            Debug.LogWarning("UnityTransport not found on NetworkManager");
            return;
        }

        utp.SetConnectionData(ip, port);
        OnLoginSuccess(loginResult, data, onSuccess);
    }
    public void OnLoginSuccess(LoginResult loginResult = null, LoginData data = null, Action<AuthResult> onSuccess = null)
    {
        onSuccess?.Invoke(new AuthResult
        {
            clientApi = clientAPI,
            userId = loginResult.PlayFabId,
            email = data.email,
            accessToken = loginResult.SessionTicket,
            message = "Đăng nhập thành công"
        });
        PlayerPrefs.SetString(EMAIL_KEY, data.email);
        PlayerPrefs.SetString(PASSWORD_KEY, data.password);
        PlayerPrefs.Save();
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
            TitleId = PlayFabSettings.TitleId,
            Email = email,
            Password = password,
        };

        var loginData = new LoginData
        {
            email = email,
            password = password,
        };

        clientAPI.LoginWithEmailAddress(
            request,
            onResSuccess =>
            {
                PlayFabAuthenticationAPI.GetEntityToken(
                    new PlayFab.AuthenticationModels.GetEntityTokenRequest(),
                    entityResult =>
                    {
                        bool hasServerInfo =
                            !string.IsNullOrEmpty(configuration.ipAddress) &&
                            configuration.port > 0;

                        if (hasServerInfo)
                        {
                            Debug.Log($"Đã có server: {configuration.ipAddress}:{configuration.port}");
                            OnPlayFabLoginSuccess(onResSuccess, loginData, onSuccess, onError);
                            return;
                        }

                        var serverRequest = new PlayFab.MultiplayerModels.RequestMultiplayerServerRequest
                        {
                            BuildId = configuration.buildId,
                            SessionId = Guid.NewGuid().ToString(),
                            PreferredRegions = new List<string> { "EastUs" }
                        };

                        PlayFabMultiplayerAPI.RequestMultiplayerServer(
                            serverRequest,
                            serverResult =>
                            {
                                configuration.ipAddress = serverResult.IPV4Address;
                                configuration.port = (ushort)serverResult.Ports[0].Num;

                                OnPlayFabLoginSuccess(onResSuccess, loginData, onSuccess, onError);
                            },
                            error =>
                            {
                                onError?.Invoke(new AuthError(
                                    "PLAYFAB_REQUEST_SERVER_FAILED",
                                    error.GenerateErrorReport()));
                            });
                    },
                    entityError =>
                    {
                        onError?.Invoke(new AuthError(
                            "PLAYFAB_ENTITY_TOKEN_FAILED",
                            entityError.GenerateErrorReport()
                        ));
                    });
            },
            onResError =>
            {
                onError?.Invoke(new AuthError(
                    "PLAYFAB_LOGIN_FAILED",
                    onResError.GenerateErrorReport()
                ));
            });
    }

}