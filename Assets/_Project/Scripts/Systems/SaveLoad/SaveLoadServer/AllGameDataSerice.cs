
using System;
using System.Collections.Generic;
using UnityEngine;

public class AllGameDataSerice : ILoadRemote<GameDataCenter>
{
    private PlayFabDataServerService service;
    private List<ILoadGameData<GameDataCenter, AllGameDataResponseDto>> loadGameDatas;

    public AllGameDataSerice(PlayFabDataServerService service)
    {
        this.service = service;
        loadGameDatas = new();
        loadGameDatas.Add(new LoadRealmDataServerSide());
        loadGameDatas.Add(new LoadEssenceAndRaceDataServerSide());
        loadGameDatas.Add(new LoadEquipmentDataServerSide());
        loadGameDatas.Add(new LoadChampionDataServerSide());
        loadGameDatas.Add(new LoadCharacterDataServerSide());
        loadGameDatas.Add(new LoadShopDataServerSide());
        loadGameDatas.Add(new LoadSpiritStoneMineDataServerSide());
        loadGameDatas.Add(new LoadDemonBeastDataServerSide());
    }

    public void LoadGame(GameDataCenter gameData, Action callback)
    {
        service.LoadAllGameData((gameDataDTO) =>
        {
            try
            {
                AllGameDataResponseDto allGameDataResponse = gameDataDTO;

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