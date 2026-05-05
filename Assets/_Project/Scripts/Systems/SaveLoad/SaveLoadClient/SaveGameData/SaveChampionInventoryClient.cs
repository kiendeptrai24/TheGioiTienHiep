using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class SaveChampionInventoryClient : ISaveGameData<GameData, PlayerClientDataDto>
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

            List<ChampionDataDto> championDataDto = new List<ChampionDataDto>();
            foreach (var item in gameData.itemDatas)
            {
                if (item is not HeroData)
                    continue;
                championDataDto.Add(DataMapper.ToDto(item as HeroData));
            }
            playerClientDataDto.championInInventoryRes.AddRange(championDataDto);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveChampionInventoryClient failed: {ex.Message}");
        }
    }
}