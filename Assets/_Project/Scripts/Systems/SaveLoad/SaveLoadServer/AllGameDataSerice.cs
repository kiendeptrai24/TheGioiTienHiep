
using System;
using System.Collections.Generic;
using UnityEngine;

public class AllGameDataSerice : ILoadRemoteServer
{
    private PlayFabDataServerService service;
    private List<ILoadGameData> loadGameDatas;
    public AllGameDataSerice(PlayFabDataServerService service)
    {
        this.service = service;
        loadGameDatas = new();
        loadGameDatas.Add(new LoadEssenceAndRaceData());
        loadGameDatas.Add(new LoadEquipmentData());
        loadGameDatas.Add(new LoadChampionData());
        loadGameDatas.Add(new LoadRealmData());
        loadGameDatas.Add(new LoadShopData());
        loadGameDatas.Add(new LoadCharacterData());
    }

    public void LoadGame(GameDataCenter gameData, Action callback)
    {
        service.LoadAllGameData((gameDataDTO) =>
        {
            try
            {
                AllGameDataResponseDto allGameDataResponse = gameDataDTO;
                if (allGameDataResponse == null)
                {
                    Debug.Log("LoadGame: itemsShop is null");
                    return;
                }
                if (allGameDataResponse.essenceAndRaceRes == null)
                {
                    Debug.Log("LoadGame: essenceAndRaceRes is null");
                    return;
                }
                if (allGameDataResponse.equipmentRes == null)
                {
                    Debug.Log("LoadGame: equipmentRes is null");
                    return;
                }
                if (allGameDataResponse.championRes == null)
                {
                    Debug.Log("LoadGame: championRes is null");
                    return;
                }
                if (allGameDataResponse.realmRes == null)
                {
                    Debug.Log("LoadGame: realmRes is null");
                    return;
                }
                if (allGameDataResponse.shopRes == null)
                {
                    Debug.Log("LoadGame: shopRes is null");
                    return;
                }
                if (allGameDataResponse.characterRes == null)
                {
                    Debug.Log("LoadGame: characterRes is null");
                    return;
                }

                foreach (var loadGameData in loadGameDatas)
                {
                    if (loadGameData == null)
                        continue;
                    loadGameData.LoadGameData(gameData, allGameDataResponse);
                }
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
            }
        });
    }
}