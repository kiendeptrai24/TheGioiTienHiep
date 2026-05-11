

using System;
using System.Collections.Generic;

public class PlayerInventoryService : ILoadRemote<GameData>, ISaveRemote<GameData>
{
    private PlayFabDataClientService service;
    private List<ILoadGameData<GameData, PlayerClientDataDto>> loadGameDatas;
    private List<ISaveGameData<GameData, PlayerClientDataDto>> saveGameDatas;
    public PlayerInventoryService(PlayFabDataClientService service)
    {
        this.service = service;
        loadGameDatas = new List<ILoadGameData<GameData, PlayerClientDataDto>>()
        {
            new LoadEquipmentInventoryClient(),
            new LoadItemUsedClient(),
            new LoadChampionInventoryClient(),
            new LoadChampionTeamClient(),
            new LoadProfileClient(),
            new LoadItemDataPointClient()
        };
        saveGameDatas = new List<ISaveGameData<GameData, PlayerClientDataDto>>()
        {
            new SaveEquipmentInventoryClient(),
            new SaveItemUsedClient(),
            new SaveChampionInventoryClient(),
            new SaveChampionTeamClient(),
            new SaveProfileClient(),
            new SaveItemDataPointClient()
        };
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadPlayerInventoryData(gameData.characterId, (gameDataDTO) =>
        {
            try
            {
                if (gameDataDTO == null)
                {
                    callback?.Invoke();
                    return;
                }
                var playerClientDataDto = new PlayerClientDataDto();
                playerClientDataDto = gameDataDTO;

                foreach (var loadGameData in loadGameDatas)
                {
                    loadGameData.LoadGameData(gameData, playerClientDataDto);
                }
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("LoadPlayerDatas Error occurred while loading player inventory data." + ex.Message);
            }
        });
    }

    public void SaveGame(GameData gameData)
    {
        var playerClientDataDto = new PlayerClientDataDto();
        foreach (var saveGameData in saveGameDatas)
        {
            saveGameData.SaveGameData(gameData, playerClientDataDto);
        }
        service.SavePlayerInventoryData(gameData, playerClientDataDto);
    }
}