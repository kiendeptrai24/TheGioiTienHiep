

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadChampionDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{

    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<ChampionDataDto> championResponse = allGameDataDto.championRes;
            if (championResponse == null)
            {
                Debug.Log("LoadGame: itemsShop is null");
                return;
            }

            List<HeroData> heroDatas = new();
            for (int i = 0; i < championResponse.Count; i++)
            {
                var heroData = DataMapper.MapChampionData(championResponse[i]);
                if (heroData == null)
                {
                    Debug.LogWarning($"LoadGame: Failed to map champion data for instanceId {championResponse[i].instanceId}");
                    continue;
                }
                heroDatas.Add(heroData);
            }
            gameData.championItems = heroDatas;
            gameData.allItems.AddRange(heroDatas);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
        }
    }

}