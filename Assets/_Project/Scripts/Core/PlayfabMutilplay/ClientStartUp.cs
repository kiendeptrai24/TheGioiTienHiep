using System;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ClientStartUp : Singleton<ClientStartUp>
{
    public Configuration configuration;
    public ServerStartUp serverStartUp;

    public void OnLoginUserButtonClick()
    {
        if (configuration.buildType == BuildType.REMOTE_CLIENT)
        {
            if (string.IsNullOrEmpty(configuration.buildId))
            {
                throw new Exception("A remote client build must have a buildId. Add it to the Configuration. Get this from your Multiplayer Game Manager in the PlayFab web console.");
            }
            else
            {
                LoginRemoteUser();
            }
        }
        else if (configuration.buildType == BuildType.LOCAL_CLIENT)
        {
            if (configuration.port == 0)
            {

            }
        }
    }
    public void OnStartLocalClientButtonClick(ushort port)
    {
        if (port == 0 || port > 65535)
        {
            Debug.LogError("Port không hợp lệ! Port phải trong khoảng 1 - 65535.");
            return;
        }

        var utp = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;

        if (utp == null)
        {
            Debug.LogError("UnityTransport không tồn tại hoặc chưa được gán!");
            return;
        }

        Debug.Log($"Client đang dùng port: {port}");

        // Set địa chỉ và port
        utp.SetConnectionData("127.0.0.1", port);
        NetworkManager.Singleton.StartClient();
    }


    public void LoginRemoteUser()
    {
        Debug.Log("[ClientStartUp].LoginRemoteUser");

        //We need to login a user to get at PlayFab API's. 
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = GUIDUtility.getUniqueID()
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
    }

    private void OnLoginError(PlayFabError response)
    {
        Debug.LogError(response.GenerateErrorReport());
    }

    private void OnPlayFabLoginSuccess(LoginResult response)
    {
        Debug.Log("[ClientStartUp].OnPlayFabLoginSuccess: " + response.PlayFabId);

        if (string.IsNullOrEmpty(configuration.ipAddress))
        {
            //We need to grab an IP and Port from a server based on the buildId. Copy this and add it to your Configuration.
            RequestMultiplayerServer();
        }
        else
        {
            ConnectRemoteClient();
        }
    }

    // Tạo một server mới (PlayFab alloc server)
    private void RequestMultiplayerServer()
    {
        Debug.Log("[ClientStartUp].RequestMultiplayerServer");

        var requestData = new RequestMultiplayerServerRequest
        {
            BuildId = configuration.buildId,
            SessionId = Guid.NewGuid().ToString(),
            PreferredRegions = new List<string> { AzureRegion.EastUs.ToString() } // có thể cho vào config
        };

        PlayFabMultiplayerAPI.RequestMultiplayerServer(
            requestData,
            OnRequestMultiplayerServer,
            OnRequestMultiplayerServerError);
    }

    private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        Debug.Log("[ClientStartUp].OnRequestMultiplayerServer: " + response.ToString());
        ConnectRemoteClient(response);
    }

    private void ConnectRemoteClient(RequestMultiplayerServerResponse response = null)
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
            Debug.LogError("UnityTransport not found on NetworkManager");
            return;
        }

        utp.SetConnectionData(ip, port);
        // NetworkManager.Singleton.StartClient();
    }

    private void OnRequestMultiplayerServerError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
