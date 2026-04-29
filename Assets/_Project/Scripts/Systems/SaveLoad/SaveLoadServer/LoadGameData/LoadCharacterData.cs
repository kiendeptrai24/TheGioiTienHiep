

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacterData : ILoadGameData
{

    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto, Action callback)
    {
        try
        {
            if (allGameDataDto == null || allGameDataDto.characterRes == null)
            {
                Debug.Log("gameDataDTO is null");
                callback?.Invoke();
                return;
            }
            var itemTeam = allGameDataDto.characterRes;
            List<ItemData> itemDatas = new();
            foreach (var item in itemTeam)
            {
                var heroData = new HeroData();
                CharacterDataDto itemDto = item;
                heroData.instanceId = itemDto.instanceId;
                heroData.itemDescription = itemDto.description;
                heroData.raceId = itemDto.raceId;
                heroData.realmId = itemDto.realmId;
                itemDatas.Add(heroData);
            }
            gameData.gameBaseCharacterDatas = itemDatas;
            callback?.Invoke();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load game base character data " + ex.Message);
        }
    }

}