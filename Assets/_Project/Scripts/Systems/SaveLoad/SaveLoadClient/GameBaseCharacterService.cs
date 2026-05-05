
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBaseCharacterService : ILoadRemote<GameData>
{
    private PlayFabDataClientService service;
    public GameBaseCharacterService(PlayFabDataClientService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadGameBaseCharacterData((gameDataDTO) =>
        {
            try
            {
                if (gameDataDTO == null)
                {
                    Debug.Log("gameDataDTO is null");
                    callback?.Invoke();
                    return;
                }
                var itemTeam = gameDataDTO;
                List<ItemData> itemDatas = new();
                foreach (var item in itemTeam.Data)
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
        });
    }
}