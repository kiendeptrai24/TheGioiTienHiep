using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacterDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{
    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<CharacterDataDto> characterResponse = allGameDataDto.characterRes;
            if (characterResponse == null)
            {
                Debug.Log("LoadGame: characterRes is null");
                return;
            }
            List<HeroData> heroDatas = new();
            for (int i = 0; i < characterResponse.Count; i++)
            {
                HeroData heroData = DataMapper.MapCharacterData(characterResponse[i]);
                if (heroData == null)
                {
                    Debug.LogWarning($"LoadGame: Failed to map character data for instanceId {characterResponse[i].instanceId}");
                    continue;
                }
                // Add to list
                heroDatas.Add(heroData);
            }
            gameData.characterDatas = heroDatas;
            gameData.allItems.AddRange(heroDatas);
        }
        catch (Exception e)
        {
            Debug.LogError("LoadGame: Failed to load character data - " + e.Message);
        }
    }
}