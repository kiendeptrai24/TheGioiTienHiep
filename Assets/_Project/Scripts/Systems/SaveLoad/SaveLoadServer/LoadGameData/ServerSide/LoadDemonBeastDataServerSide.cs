using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadDemonBeastDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{
    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<DemonBeastDataDto> demonBeastItems = allGameDataDto.demonBeastRes;
            if (demonBeastItems == null)
            {
                Debug.Log("LoadGame: demonBeastRes is null");
                return;
            }

            for (int i = 0; i < demonBeastItems.Count; i++)
            {
                var demonBeastData = DataMapper.MapDemonBeastData(demonBeastItems[i]);
                if (demonBeastData == null)
                {
                    Debug.LogWarning($"LoadGame: Failed to map demon beast data for instanceId {demonBeastItems[i].instanceId}");
                    continue;
                }
                gameData.demonBeastItems.Add(demonBeastData);
                gameData.allItems.Add(demonBeastData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load demon beast data " + ex.Message);
        }
    }

}