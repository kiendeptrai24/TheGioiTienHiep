using System;
using System.Collections.Generic;
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
                var heroData = DataMapper.ToDto(item as HeroData);
                if (heroData == null)
                    continue;
                championDataDto.Add(heroData);
            }
            playerClientDataDto.championInInventoryRes.AddRange(championDataDto);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveChampionInventoryClient failed: {ex.Message}");
        }
    }
}