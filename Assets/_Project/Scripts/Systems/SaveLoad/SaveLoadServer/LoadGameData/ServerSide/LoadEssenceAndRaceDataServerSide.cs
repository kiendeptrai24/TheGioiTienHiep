

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadEssenceAndRaceDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{
    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<EssenceAndRaceDataDto> essenceAndRaceItems = allGameDataDto.essenceAndRaceRes;
            if (essenceAndRaceItems == null)
            {
                Debug.Log("LoadGame: essenceAndRaceRes is null");
                return;
            }
            for (int i = 0; i < essenceAndRaceItems.Count; i++)
            {
                var dto = essenceAndRaceItems[i];
                if(dto.type == EssenceAndRaceType.Essence)
                {
                    var essenceData = DataMapper.MapEssenceData(dto);
                    if (essenceData == null)
                    {
                        Debug.LogWarning($"LoadGame: Failed to map essence data for instanceId {dto.instanceId}");
                        continue;
                    }
                    gameData.essenceItems.Add(essenceData);
                    gameData.allItems.Add(essenceData);
                }
                else if(dto.type == EssenceAndRaceType.Race)
                {
                    var raceData = DataMapper.MapRaceData(dto);
                    if (raceData == null)
                    {
                        Debug.LogWarning($"LoadGame: Failed to map race data for instanceId {dto.instanceId}");
                        continue;
                    }
                    gameData.raceItems.Add(raceData);
                    gameData.allItems.Add(raceData);
                }
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load essence and race data " + ex.Message);
        }
    }

}