using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadSpiritStoneMineDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{
    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<SpiritStoneMineDataDto> resourceItems = allGameDataDto.spiritStoneMineRes;
            if (resourceItems == null)
            {
                Debug.Log("LoadGame: spiritStoneMineRes is null");
                return;
            }

            for (int i = 0; i < resourceItems.Count; i++)
            {
                var resourceData = DataMapper.MapSpiritStoneMineData(resourceItems[i]);
                if (resourceData == null)
                {
                    Debug.LogWarning($"LoadGame: Failed to map resource data for instanceId {resourceItems[i].instanceId}");
                    continue;
                }
                gameData.spiritStoneMineItems.Add(resourceData);
                gameData.allItems.Add(resourceData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load resource data " + ex.Message);
        }
    }

}