using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveItemUsedClient : ISaveGameData<GameData, PlayerClientDataDto>
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
            foreach (var item in gameData.itemUsedDatas)
            {
                itemDataDto.Add(DataMapper.ToDto(item));
            }
            playerClientDataDto.itemUsedRes.AddRange(itemDataDto);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveItemUsedClient failed: {ex.Message}");
        }
    }
}