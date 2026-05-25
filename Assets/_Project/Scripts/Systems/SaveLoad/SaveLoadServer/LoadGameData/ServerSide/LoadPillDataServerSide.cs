using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadPillDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{
    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<PillDataDto> pillItems = allGameDataDto.pillRes;
            if (pillItems == null)
            {
                Debug.Log("LoadGame: demonBeastRes is null");
                return;
            }

            for (int i = 0; i < pillItems.Count; i++)
            {
                PillData pillData = DataMapper.MapPillData(pillItems[i]);
                if (pillData == null)
                {
                    Debug.LogWarning($"LoadGame: Failed to map demon beast data for instanceId {pillItems[i].instanceId}");
                    continue;
                }
                gameData.pillDatas.Add(pillData);
                gameData.allItems.Add(pillData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load demon beast data " + ex.Message);
        }
    }

}