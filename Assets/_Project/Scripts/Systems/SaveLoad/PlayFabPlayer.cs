
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using UnityEngine;

public class PlayFabPlayer
{
    public bool loggedIn = false;
    public bool dataLoading = false;
    public bool dataLoaded = false;
    public string PlayFabId;
    public Dictionary<string, ObjectResult> playerData;
    public Dictionary<string, UserDataRecord> data;

    private PlayFabClientInstanceAPI clientApi;
    private PlayFabDataInstanceAPI dataApi;

    public void Login(string customId, Action<PlayFabClientInstanceAPI> callback)
    {
        clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);

        var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };

        clientApi.LoginWithCustomID(request, result =>
        {
            PlayFabId = result.PlayFabId;
            loggedIn = true;
            dataApi = new PlayFabDataInstanceAPI(clientApi.authenticationContext);
            callback?.Invoke(clientApi);
            Debug.Log("Login call succeeded.");
        }, error =>
        {
            Debug.LogWarning("Something went wrong with the login call.");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LoadData(Action<PlayerDataDTO> callback)
    {
        clientApi.GetTitleData(new GetTitleDataRequest(),
        r =>
        {
            if (r.Data != null && r.Data.ContainsKey("inventory"))
            {
                string json = r.Data["inventory"];

                PlayerDataDTO item = JsonConvert.DeserializeObject<PlayerDataDTO>(json);
                callback?.Invoke(item);
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LoadShopData(Action<PlayerDataDTO> callback)
    {
        clientApi.GetTitleData(new GetTitleDataRequest(),
        r =>
        {
            if (r.Data != null && r.Data.ContainsKey("shop"))
            {
                string json = r.Data["shop"];

                PlayerDataDTO item = JsonConvert.DeserializeObject<PlayerDataDTO>(json);

                callback?.Invoke(item);
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LoadPlayerData(Action<PlayerDataDTO> callback)
    {
        clientApi.GetUserData(new GetUserDataRequest(),
        result =>
        {
            if (result.Data != null)
            {
                var r = result;
                if (r.Data != null && r.Data.ContainsKey("inventory"))
                {
                    string json = r.Data["inventory"].Value;

                    PlayerDataDTO item = JsonConvert.DeserializeObject<PlayerDataDTO>(json);
                    callback?.Invoke(item);
                }
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void SetData(PlayerDataDTO items)
    {
        string key = "inventory";

        string json = JsonConvert.SerializeObject(items);
        clientApi.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, json } }
        }, r => {}, e => Debug.LogError(e.GenerateErrorReport()));
    }
}