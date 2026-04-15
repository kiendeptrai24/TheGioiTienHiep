using System;
using System.Collections.Generic;
using System.Linq;
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

    public void LoadPlayerHeroData(string characterId, Action<HeroDataDTO> callback)
    {
        LoadUserData($"hero inventory {characterId}", callback);
    }
    public void LoadPlayerData(string characterId, Action<ItemDataDTO> callback)
    {
        LoadUserData($"inventory {characterId}", callback);
    }
    public void LoadPlayerDatasUsed(string characterId, Action<ItemDataDTO> callback)
    {
        LoadUserData($"inventory used {characterId}", callback);
    }

    public void LoadTeamData(string characterId, Action<HeroInTeamDataDTO> callback)
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
    public void SavePlayerHeroData(GameData gameData)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("LoadPlayerHeroData failed: gameData is null");
                return;
            }

            if (gameData == null)
            {
                Debug.LogError("SetTeamData failed: gameData is null");
                return;
            }

            List<HeroData> listItemData = gameData.itemDatas
            .OfType<HeroData>()
            .ToList();
            if (listItemData == null)
            {
                Debug.LogError("null or empty");
                return;
            }
            HeroDataDTO inventoryData = new HeroDataDTO
            {
                inventoryItems = listItemData ?? new List<HeroData>()
            };

            SaveUserData($"hero inventory {gameData.characterId}", inventoryData);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
        }
    }
    public void SetTeamData(GameData gameData)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("SetTeamData failed: gameData is null");
                return;
            }

            HeroInTeamDataDTO teamData = new HeroInTeamDataDTO();

            if (gameData.itemDatasInTeam != null)
            {
                foreach (var item in gameData.itemDatasInTeam)
                {
                    teamData.inventoryItems.Add(item as HeroData);

                    if (item is HeroData heroData)
                    {
                        teamData.championsIndex.Add(heroData.championIndex);
                    }
                }
            }

            SaveUserData($"team {gameData.characterId}", teamData);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
        }
    }

    public void SetItemInventoryData(GameData gameData)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("SetItemInventoryData failed: gameData is null");
                return;
            }
            var listItemData = gameData.itemDatas?.FindAll(x => x is not HeroData) ?? new List<ItemData>();

            if (listItemData == null)
            {
                Debug.LogError("null or empty");
                return;
            }

            ItemDataDTO inventoryData = new ItemDataDTO
            {
                inventoryItems = listItemData
            };

            SaveUserData($"inventory {gameData.characterId}", inventoryData);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
        }
    }
    public void SetItemInventoryDataUsed(GameData gameData)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("SetItemInventoryData failed: gameData is null");
                return;
            }
            if (gameData.itemDatasUsed == null)
            {
                Debug.Log("null or empty");
                return;
            }
            List<ItemData> listItemData = gameData.itemDatasUsed;

            if (listItemData == null)
            {
                Debug.LogError("null or empty");
                return;
            }

            ItemDataDTO inventoryData = new ItemDataDTO
            {
                inventoryItems = listItemData
            };

            SaveUserData($"inventory used {gameData.characterId}", inventoryData);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
        }
    }
    public void SetItemCharacter(GameData gameData)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("SetItemCharacter failed: gameData is null");
                return;
            }

            ItemCharacterDataDTO inventoryData = new ItemCharacterDataDTO();
            if (gameData.itemDatasCharacter != null)
            {
                inventoryData.inventoryItems = new List<HeroData>();
                inventoryData.characterNames = new List<string>();
                inventoryData.characterIds = new List<string>();
                foreach (var item in gameData.itemDatasCharacter)
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
            SaveUserData("character", inventoryData);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
        }

    }
    public void SetProfile(GameData gameData)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("SetProfile failed: gameData is null");
                return;
            }
            Vector3DTO posDTO = new Vector3DTO(gameData.position);

            Vector3 rot = new Vector3(gameData.rotation.eulerAngles.x, gameData.rotation.eulerAngles.x, gameData.rotation.eulerAngles.x);

            Vector3DTO rotDTO = new Vector3DTO(rot);

            ItemDataPoint itemDataPoint = gameData.itemDataPoint;

            if (itemDataPoint == null)
            {
                itemDataPoint = new ItemDataPoint();
            }
            PlayerProfileDTO profile = new PlayerProfileDTO
            {
                characterId = gameData.characterId,
                coins = gameData.coins,
                playerName = gameData.characterName,
                position = posDTO,
                rotation = rotDTO,
                potentialPoint = gameData.potentialPoint,
                skillPoint = gameData.skillPoint,
                itemDataPoint = itemDataPoint,
                // ===== OFFLINE MINING SAVE =====
                mineOfflineDataList = gameData.mineOfflineDataList ?? new MineOfflineDataList()
            };

            SaveUserData($"profile {gameData.characterId}", profile);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
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
            },
            error =>
            {
                Debug.LogError($"UpdateUserData Error at key '{key}': {error.GenerateErrorReport()}");
            });
        }
        catch (System.Exception)
        {
            throw;
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