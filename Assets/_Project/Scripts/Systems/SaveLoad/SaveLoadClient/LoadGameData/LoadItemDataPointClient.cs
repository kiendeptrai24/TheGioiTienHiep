using System;
using UnityEngine;

public class LoadItemDataPointClient : ILoadGameData<GameData, PlayerClientDataDto>
{
    public void LoadGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (playerClientDataDto == null || playerClientDataDto.itemDataPointRes == null)
            {
                Debug.LogWarning("LoadProfileClient: profileRes is null");
                return;
            }
            var itemDataPointDto = playerClientDataDto.itemDataPointRes;
            var itemDataPoint = DataMapper.MapItemDataPoint(itemDataPointDto).Clone() as ItemDataPoint;
            if (itemDataPoint == null) return;
            gameData.itemDataPoint = itemDataPoint;
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadProfileClient failed: {ex.Message}");
        }
    }
}