using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabDataClientService
{
    private readonly PlayFabClientInstanceAPI clientApi;

    public PlayFabDataClientService(PlayFabClientInstanceAPI clientApi)
    {
        this.clientApi = clientApi;
    }

    #region Public Load Methods

    public void LoadGameBaseCharacterData(Action<CharacterResponseDto> callback)
    {
        LoadTitleData("character", callback);
    }
    public void LoadPlayerInventoryData(string characterId, Action<PlayerClientDataDto> callback)
    {
        LoadUserData($"inventory {characterId}", callback);
    }
    public void LoadCharacter(Action<ItemCharacterDataDTO> callback)
    {
        LoadUserData("character", callback);
    }
    #endregion

    #region Public Save Methods
    public void SetItemCharacter(GameData gameData, Action<bool> onCompleted = null)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("SetItemCharacter failed: gameData is null");
                onCompleted?.Invoke(false);
                return;
            }

            ItemCharacterDataDTO inventoryData = new ItemCharacterDataDTO();
            if (gameData.itemCharacterDatas != null)
            {
                inventoryData.inventoryItems = new List<HeroData>();
                inventoryData.characterNames = new List<string>();
                inventoryData.characterIds = new List<string>();
                foreach (var item in gameData.itemCharacterDatas)
                {
                    var itemHero = item as HeroData;

                    inventoryData.inventoryItems.Add(itemHero);
                    inventoryData.characterNames.Add(item.itemName);
                    if (item is HeroData heroData)
                    {
                        inventoryData.characterIds.Add(heroData.characterId);
                    }
                    else
                    {
                        Debug.Log("SetItemCharacter: item is not HeroData");
                    }
                }
            }
            else
            {
                inventoryData.inventoryItems = new List<HeroData>();
                inventoryData.characterNames = new List<string>();
                inventoryData.characterIds = new List<string>();
            }
            SaveUserData("character", inventoryData, onCompleted);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
            onCompleted?.Invoke(false);
        }

    }

    public void SavePlayerInventoryData(GameData gameData, PlayerClientDataDto playerClientDataDto, Action<bool> onCompleted = null)
    {
        try
        {
            SaveUserData($"inventory {gameData.characterId}", playerClientDataDto, onCompleted);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
            onCompleted?.Invoke(false);
        }
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

    private void SaveUserData<T>(string key, T data, Action<bool> onCompleted = null)
    {
        string json;

        try
        {
            json = JsonConvert.SerializeObject(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Serialize Error at key '{key}': {ex}");
            onCompleted?.Invoke(false);
            return;
        }
        try
        {
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
                onCompleted?.Invoke(true);
            },
            error =>
            {
                Debug.LogError($"UpdateUserData Error at key '{key}': {error.GenerateErrorReport()}");
                onCompleted?.Invoke(false);
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error occurred while saving data at key '{key}': {ex}");
            onCompleted?.Invoke(false);
        }
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
