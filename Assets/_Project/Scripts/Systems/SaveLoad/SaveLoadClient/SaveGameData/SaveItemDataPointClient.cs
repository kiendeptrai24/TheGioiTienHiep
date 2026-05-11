

using System;
using UnityEngine;

public class SaveItemDataPointClient : ISaveGameData<GameData, PlayerClientDataDto>
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
            var itemDataPointDto = DataMapper.ToDto(gameData.itemDataPoint);
            if(itemDataPointDto == null) return;
            playerClientDataDto.itemDataPointRes = itemDataPointDto;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveItemDataPointClient failed: {ex.Message}");
        }
    }
}