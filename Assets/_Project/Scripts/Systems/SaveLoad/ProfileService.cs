
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
        service.LoadProfile(gameData.characterId, (profileDataDTO) =>
        {
            if (profileDataDTO == null)
            {
                callback?.Invoke();
                return;
            }
            if (string.IsNullOrEmpty(profileDataDTO.playerName) == false)
                gameData.characterName = profileDataDTO.playerName;
            if (string.IsNullOrEmpty(profileDataDTO.characterId) == false)
                gameData.characterId = profileDataDTO.characterId;
            gameData.coins = profileDataDTO.coins;

            // ===== LOAD OFFLINE MINING DATA =====
            if (profileDataDTO.mineOfflineDataList != null && profileDataDTO.mineOfflineDataList.Count > 0)
            {
                gameData.mineOfflineDataList = profileDataDTO.mineOfflineDataList;
            }

            callback?.Invoke();
        });
    }

    public void SaveGame(GameData gameData)
    {
        service.SetProfile(gameData);
    }
}