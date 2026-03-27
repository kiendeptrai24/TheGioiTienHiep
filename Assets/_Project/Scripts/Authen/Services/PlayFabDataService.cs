using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabDataService
{
    private readonly PlayFabClientInstanceAPI clientApi;

    public PlayFabDataService(PlayFabClientInstanceAPI clientApi)
    {
        this.clientApi = clientApi;
    }

    #region Public Load Methods

    public void LoadData(Action<ItemDataDTO> callback)
    {
        LoadTitleData("inventory", callback);
    }

    public void LoadShopData(Action<ItemDataDTO> callback)
    {
        LoadTitleData("shop", callback);
    }

    public void LoadPlayerData(string characterId, Action<ItemDataDTO> callback)
    {
        LoadUserData($"inventory {characterId}", callback);
    }

    public void LoadTeamData(string characterId, Action<HeroDataDTO> callback)
    {
        LoadUserData($"team {characterId}", callback);
    }

    public void LoadProfile(string characterId, Action<PlayerProfileDTO> callback)
    {
        LoadUserData($"profile {characterId}", callback);
    }
    public void LoadCharacter(Action<ItemCharacterDataDTO> callback)
    {
        LoadUserData("character", callback);
    }
    #endregion

    #region Public Save Methods

    public void SetTeamData(GameData gameData)
    {
        if (gameData == null)
        {
            Debug.LogError("SetTeamData failed: gameData is null");
            return;
        }

        HeroDataDTO teamData = new HeroDataDTO();

        if (gameData.itemDatasInTeam != null)
        {
            foreach (var item in gameData.itemDatasInTeam)
            {
                teamData.inventoryItems.Add(item);

                if (item is HeroData heroData)
                {
                    teamData.championsIndex.Add(heroData.championIndex);
                }
            }
        }

        SaveUserData($"team {gameData.characterId}", teamData);
    }

    public void SetItemInventoryData(GameData gameData)
    {
        if (gameData == null)
        {
            Debug.LogError("SetItemInventoryData failed: gameData is null");
            return;
        }

        ItemDataDTO inventoryData = new ItemDataDTO
        {
            inventoryItems = gameData.itemDatas ?? new List<ItemData>()
        };

        SaveUserData($"inventory {gameData.characterId}", inventoryData);
    }
    public void SetItemCharacter(GameData gameData)
    {
        if (gameData == null)
        {
            Debug.LogError("SetItemCharacter failed: gameData is null");
            return;
        }

        ItemCharacterDataDTO inventoryData = new ItemCharacterDataDTO();
        if (gameData.itemDatasCharacter != null)
        {
            foreach (var item in gameData.itemDatasCharacter)
            {
                inventoryData.inventoryItems.Add(item);
                inventoryData.characterNames.Add(item.itemName);
                if (item is HeroData heroData)
                {
                    inventoryData.characterIds.Add(heroData.characterId);
                }
            }
        }
        else
        {
            inventoryData.inventoryItems = new List<ItemData>();
            inventoryData.characterNames = new List<string>();
        }
        SaveUserData("character", inventoryData);

    }
    public void SetProfile(GameData gameData)
    {
        if (gameData == null)
        {
            Debug.LogError("SetProfile failed: gameData is null");
            return;
        }

        PlayerProfileDTO profile = new PlayerProfileDTO
        {
            characterId = gameData.characterId,
            coins = gameData.coins,
            playerName = gameData.characterName,
            // ===== OFFLINE MINING SAVE =====
            mineOfflineDataList = gameData.mineOfflineDataList ?? new MineOfflineDataList()
        };

        SaveUserData($"profile {gameData.characterId}", profile);
    }

    #endregion

    #region Private Generic Load/Save

    private void LoadTitleData<T>(string key, Action<T> callback)
    {
        clientApi.GetTitleData(
            new GetTitleDataRequest(),
            result =>
            {
                if (result.Data == null)
                {
                    Debug.LogWarning($"LoadTitleData<{typeof(T).Name}> failed: TitleData is null");
                    callback?.Invoke(default);
                    return;
                }

                if (!result.Data.TryGetValue(key, out string json) || string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning($"LoadTitleData<{typeof(T).Name}> failed: key '{key}' not found");
                    callback?.Invoke(default);
                    return;
                }

                TryDeserialize(json, callback, key);
            },
            error =>
            {
                Debug.LogError($"GetTitleData Error: {error.GenerateErrorReport()}");
                callback?.Invoke(default);
            });
    }

    private void LoadUserData<T>(string key, Action<T> callback)
    {
        clientApi.GetUserData(
            new GetUserDataRequest(),
            result =>
            {
                if (result.Data == null)
                {
                    Debug.LogWarning($"LoadUserData<{typeof(T).Name}> failed: UserData is null");
                    callback?.Invoke(default);
                    return;
                }

                if (!result.Data.TryGetValue(key, out var userDataRecord) || userDataRecord == null || string.IsNullOrEmpty(userDataRecord.Value))
                {
                    Debug.LogWarning($"LoadUserData<{typeof(T).Name}> failed: key '{key}' not found");
                    callback?.Invoke(default);
                    return;
                }

                TryDeserialize(userDataRecord.Value, callback, key);
            },
            error =>
            {
                Debug.LogError($"GetUserData Error: {error.GenerateErrorReport()}");
                callback?.Invoke(default);
            });
    }

    private void SaveUserData<T>(string key, T data)
    {
        string json;

        try
        {
            json = JsonConvert.SerializeObject(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Serialize Error at key '{key}': {ex}");
            return;
        }

        clientApi.UpdateUserData(
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { key, json }
                }
            },
            result =>
            {
                Debug.Log($"SaveUserData success: key = {key}");
            },
            error =>
            {
                Debug.LogError($"UpdateUserData Error at key '{key}': {error.GenerateErrorReport()}");
            });
    }

    private void TryDeserialize<T>(string json, Action<T> callback, string key)
    {
        try
        {
            T data = JsonConvert.DeserializeObject<T>(json);
            callback?.Invoke(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Deserialize Error at key '{key}': {ex}");
            callback?.Invoke(default);
        }
    }

    #endregion
}