
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using PlayFab.Internal;
using UnityEngine;

public class PlayFabPlayer
{
    public bool loggedIn = false;
    public string PlayFabId;

    private PlayFabClientInstanceAPI clientApi;

    public void Login(string customId, Action<PlayFabClientInstanceAPI> callback)
    {
        clientApi = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);

        var request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };

        clientApi.LoginWithCustomID(request, result =>
        {
            PlayFabId = result.PlayFabId;
            loggedIn = true;
            callback?.Invoke(clientApi);
            Debug.Log("Login call succeeded.");
        }, error =>
        {
            Debug.LogWarning("Something went wrong with the login call.");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void Login(AuthResult result)
    {
        loggedIn = true;
        clientApi = result.clientApi;
        PlayFabId = result.userId;
    }
    public void Logout()
    {

    }
    public void LoadData(Action<ItemDataDTO> callback)
    {
        clientApi.GetTitleData(new GetTitleDataRequest(),
        r =>
        {
            if (r.Data != null && r.Data.ContainsKey("inventory"))
            {
                string json = r.Data["inventory"];

                ItemDataDTO item = JsonConvert.DeserializeObject<ItemDataDTO>(json);
                callback?.Invoke(item);
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LoadTeamData(Action<HeroDataDTO> callback)
    {
        clientApi.GetUserData(new GetUserDataRequest(),
        result =>
        {
            if (result.Data != null)
            {
                var r = result;
                if (r.Data != null && r.Data.ContainsKey("team"))
                {
                    string json = r.Data["team"].Value;

                    HeroDataDTO item = JsonConvert.DeserializeObject<HeroDataDTO>(json);

                    callback?.Invoke(item);
                }
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void SetTeamData(GameData gameData)
    {
        string key = "team";
        HeroDataDTO items = new HeroDataDTO();
        foreach (var item in gameData.itemDatasInTeam)
        {
            items.inventoryItems.Add(item);
            if (item is HeroData heroData)
            {
                items.championsIndex.Add(heroData.championIndex);
            }
        }

        string json = JsonConvert.SerializeObject(items);
        clientApi.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, json } }
        }, r => { }, e => Debug.LogError(e.GenerateErrorReport()));
    }
    public void LoadShopData(Action<ItemDataDTO> callback)
    {
        clientApi.GetTitleData(new GetTitleDataRequest(),
        r =>
        {
            if (r.Data != null && r.Data.ContainsKey("shop"))
            {
                string json = r.Data["shop"];

                ItemDataDTO item = JsonConvert.DeserializeObject<ItemDataDTO>(json);

                callback?.Invoke(item);
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LoadPlayerData(Action<ItemDataDTO> callback)
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

                    ItemDataDTO item = JsonConvert.DeserializeObject<ItemDataDTO>(json);
                    callback?.Invoke(item);
                }
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void SetItemInvenoryData(GameData gameData)
    {
        string key = "inventory";
        ItemDataDTO items = new ItemDataDTO();
        items.inventoryItems = gameData.itemDatas;

        string json = JsonConvert.SerializeObject(items);
        clientApi.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, json } }
        }, r => { }, e => Debug.LogError(e.GenerateErrorReport()));
    }
    public void LoadProfile(Action<PlayerProfileDTO> callback)
    {
        clientApi.GetUserData(new GetUserDataRequest(),
        result =>
        {
            if (result.Data != null)
            {
                var r = result;
                if (r.Data != null && r.Data.ContainsKey("profile"))
                {
                    string json = r.Data["profile"].Value;

                    PlayerProfileDTO item = JsonConvert.DeserializeObject<PlayerProfileDTO>(json);
                    callback?.Invoke(item);
                }
            }
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void SetProfile(GameData gameData)
    {
        string key = "profile";
        PlayerProfileDTO profile = new PlayerProfileDTO();
        profile.coins = gameData.coins;
        profile.playerName = gameData.playerName;


        string json = JsonConvert.SerializeObject(profile);
        clientApi.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { key, json } }
        }, r => { }, e => Debug.LogError(e.GenerateErrorReport()));
    }
}