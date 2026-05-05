using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadItemUsedClient : ILoadGameData<GameData, PlayerClientDataDto>
{
    public void LoadGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (playerClientDataDto == null || playerClientDataDto.itemUsedRes == null)
            {
                Debug.LogWarning("LoadItemUsedClient: itemUsedRes is null");
                return;
            }

            var dataManager = GameDataCenterManager.Instance;
            foreach (var itemDto in playerClientDataDto.itemUsedRes)
            {
                var itemData = dataManager.GetItemById(itemDto.instanceId);
                if (itemData != null)
                {
                    gameData.itemUsedDatas.Add(itemData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadItemUsedClient failed: {ex.Message}");
        }
    }
}