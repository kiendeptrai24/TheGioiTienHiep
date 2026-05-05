using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveEquipmentInventoryClient : ISaveGameData<GameData, PlayerClientDataDto>
{
    public void SaveGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("LoadPlayerHeroData failed: gameData is null");
                return;
            }
            List<ItemDataDto> itemDataDto = new List<ItemDataDto>();
            foreach (var item in gameData.itemDatas)
            {
                if (item is HeroData)
                        continue;
                itemDataDto.Add(DataMapper.ToDto(item));
            }
            playerClientDataDto.equipmentRes.AddRange(itemDataDto);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveEquipmentInventoryClient failed: {ex.Message}");
        }
    }
}