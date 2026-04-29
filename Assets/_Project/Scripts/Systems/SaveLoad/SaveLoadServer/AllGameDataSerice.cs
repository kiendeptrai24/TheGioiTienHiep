
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
        loadGameDatas.Add(new LoadChampionData());
        loadGameDatas.Add(new LoadEquipmentData());
        loadGameDatas.Add(new LoadEssenceAndRaceData());
        loadGameDatas.Add(new LoadRealmData());
        loadGameDatas.Add(new LoadShopData());
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
                foreach (var loadGameData in loadGameDatas)
                {
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