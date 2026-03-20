
using System;
using UnityEngine;

public class ProfileService : ISaveLoadRemote
{
    private PlayFabDataService service;

    public ProfileService(PlayFabDataService service)
    {
        this.service = service;
    }
    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadProfile(gameData.playerName, (profileDataDTO) =>
        {
            if (profileDataDTO == null)
            {
                callback?.Invoke();
                return;
            }
            if (string.IsNullOrEmpty(profileDataDTO.playerName) == false)
                gameData.playerName = profileDataDTO.playerName;
            gameData.coins = profileDataDTO.coins;
            callback?.Invoke();
        });
    }

    public void SaveGame(GameData gameData)
    {
        service.SetProfile(gameData);
    }
}