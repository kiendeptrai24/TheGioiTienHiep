using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveChampionTeamClient : ISaveGameData<GameData, PlayerClientDataDto>
{
    public void SaveGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        Debug.Log("SaveChampionTeamClient: Starting save operation");
        try
        {
            if (gameData == null)
            {
                Debug.LogError("LoadPlayerHeroData failed: gameData is null");
                return;
            }
            List<ChampionDataDto> championDataDto = new List<ChampionDataDto>();
            foreach (var item in gameData.itemInTeamDatas)
            {
                var heroData = item as HeroData;
                if (heroData == null)
                {
                    Debug.LogError($"SaveChampionTeamClient: Item with id {item.instanceId} is not HeroData");
                    continue;
                }
                var heroDataDto = DataMapper.ToDto(heroData);
                if (heroDataDto == null)
                {
                    Debug.LogError($"SaveChampionTeamClient: Failed to convert HeroData with id {heroData.instanceId} to DTO");
                    continue;
                }
                heroDataDto.posX = heroData.championIndex.x;
                heroDataDto.posY = heroData.championIndex.y;
                championDataDto.Add(heroDataDto);
            }
            playerClientDataDto.championInTeamRes.AddRange(championDataDto);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveChampionTeamClient failed: {ex.Message}");
        }
    }
}