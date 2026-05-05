using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadEquipmentInventoryClient : ILoadGameData<GameData, PlayerClientDataDto>
{
    public void LoadGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (playerClientDataDto == null || playerClientDataDto.equipmentRes == null)
            {
                Debug.LogWarning("LoadEquipmentInventoryClient: equipmentRes is null");
                return;
            }

            var dataManager = GameDataCenterManager.Instance;
            foreach (var itemDto in playerClientDataDto.equipmentRes)
            {
                var itemData = dataManager.GetItemById(itemDto.instanceId);
                if (itemData != null)
                {
                    gameData.itemDatas.Add(itemData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadEquipmentInventoryClient failed: {ex.Message}");
        }
    }
}