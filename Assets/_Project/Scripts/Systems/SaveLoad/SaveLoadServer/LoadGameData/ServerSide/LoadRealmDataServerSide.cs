

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadRealmDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{

    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<ItemRealmDataDto> realmItems = allGameDataDto.realmRes;
            if (realmItems == null)
            {
                Debug.Log("LoadGame: realmRes is null");
                return;
            }
            for (int i = 0; i < realmItems.Count; i++)
            {
                var realmData = DataMapper.MapRealmData(realmItems[i]);
                if (realmData == null)
                {
                    Debug.LogWarning($"LoadGame: Failed to map realm data for instanceId {realmItems[i].instanceId}");
                    continue;
                }
                gameData.realmItems.Add(realmData);
                gameData.allItems.Add(realmData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load realm data " + ex.Message);
        }
    }

}