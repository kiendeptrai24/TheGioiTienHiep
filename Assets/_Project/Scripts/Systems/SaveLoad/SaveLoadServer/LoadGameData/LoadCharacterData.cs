using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacterData : ILoadGameData
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

            if (characterResponse == null)
            {
                Debug.Log("gameDataDTO is null");
                return;
            }
            var characters = characterResponse;
            List<HeroData> characterDatas = new();
            foreach (var character in characters)
            {
                var heroData = new HeroData();
                CharacterDataDto itemDto = character;
                heroData.instanceId = itemDto.instanceId;
                heroData.itemDescription = itemDto.description;
                heroData.raceId = itemDto.raceId;
                heroData.realmId = itemDto.realmId;
                characterDatas.Add(heroData);
            }
            gameData.characterDatas = characterDatas;
            gameData.allItems.AddRange(characterDatas);
        }
        catch (Exception e)
        {
            Debug.LogError("LoadGame: Failed to load character data - " + e.Message);
        }
    }
}