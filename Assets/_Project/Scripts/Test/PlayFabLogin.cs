using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class PlayFabLogin : MonoBehaviour
{
    public PlayFabPlayer player = new PlayFabPlayer();
    PlayFabClientInstanceAPI clientApi;
    public ItemDataDTO itemsData;
    public GameData gameData = new GameData();
    public string playerLoginId = "testLogin1";
    public Action<GameData> OnLoadGameFormPlayfab;
    public List<ISaveLoadRemote> saveLoadRemotes = new List<ISaveLoadRemote>();

    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "";
        }
        player.Login(playerLoginId, (clientApi) =>
        {
            this.clientApi = clientApi;
            UpdateDisplayName();
            saveLoadRemotes.Add(new ProfileService(this));
            saveLoadRemotes.Add(new PlayerItemInventoryService(this));
            saveLoadRemotes.Add(new ShopService(this));
            saveLoadRemotes.Add(new InventoryService(this));
            saveLoadRemotes.Add(new TeamInventoryService(this));
            LoadGameData();
        });
    }

    private void LoadGameData()
    {
        int total = saveLoadRemotes.Count;
        int completed = 0;

        foreach (var item in saveLoadRemotes)
        {
            item.LoadGame(gameData, () =>
            {
                completed++;
                if (completed == total)
                {
                    OnLoadGameFormPlayfab?.Invoke(this.gameData);
                }
            });
        }
    }
    public void SaveGameData()
    {
        foreach (var item in saveLoadRemotes)
        {
            item.SaveGame(gameData);
        }
    }
    private void UpdateDisplayName()
    {
        clientApi.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = "Kiên ngô"
        }, result =>
        {
            Debug.Log("The player's display name is now: " + result.DisplayName);
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    [ContextMenu("Get Player Profile")]
    public void GetPlayerProfile()
    {
        clientApi.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playerLoginId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result =>
        {

            Debug.Log("The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName);
        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }
}
