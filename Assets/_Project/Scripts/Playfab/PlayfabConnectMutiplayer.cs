

using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Multiplayer;
using PlayFab.MultiplayerModels;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
public class PlayfabConnectMutiplayerData
{
    public string ipAddress;
    public ushort port;
    public bool success;
}
public class PlayfabConnectMutiplayer
{
    private readonly PlayFabMultiplayerInstanceAPI _multiplayerApi;

    public PlayfabConnectMutiplayer(PlayFabAuthenticationContext authContext)
    {
        _multiplayerApi = new PlayFabMultiplayerInstanceAPI(authContext);
    }
    public void RequestMultiplayerServer(PlayFabClientInstanceAPI clientAPI, Configuration configuration, Action<PlayfabConnectMutiplayerData> result)
    {
        Debug.Log("[ClientStartUp].RequestMultiplayerServer");

        var requestData = new RequestMultiplayerServerRequest
        {
            BuildId = configuration.buildId,
            SessionId = Guid.NewGuid().ToString(),
            PreferredRegions = new List<string> { AzureRegion.EastUs.ToString() }
        };
        
        _multiplayerApi.RequestMultiplayerServer(
            requestData,
            resultCallback =>
            {
                result?.Invoke(new PlayfabConnectMutiplayerData
                {
                    ipAddress = resultCallback.IPV4Address,
                    port = (ushort)resultCallback.Ports[0].Num,
                    success = true
                });
            },
            errorCallback =>
            {
                OnRequestMultiplayerServerError(errorCallback);
            });
    }
    private void OnRequestMultiplayerServerError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}