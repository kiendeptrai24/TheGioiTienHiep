
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
            try
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


                gameData.position = profileDataDTO.position.ToVector3();
                gameData.rotation = Quaternion.LookRotation(profileDataDTO.rotation.ToVector3());
                gameData.point = profileDataDTO.point;
                gameData.itemDataPoint = profileDataDTO.itemDataPoint;
                if (profileDataDTO.itemDataPoint == null)
                {
                    gameData.itemDataPoint = new ItemDataPoint();
                }
                if (profileDataDTO.position.Equals(default(Vector3DTO)))
                {
                    gameData.position = new Vector3(500, 0, 440);
                }
                if (profileDataDTO.rotation.Equals(default(Vector3DTO)))
                {
                    gameData.rotation = Quaternion.identity;
                }

                // ===== LOAD OFFLINE MINING DATA =====
                if (profileDataDTO.mineOfflineDataList != null && profileDataDTO.mineOfflineDataList.Count > 0)
                {
                    gameData.mineOfflineDataList = profileDataDTO.mineOfflineDataList;
                }

                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadGame: Failed to load profile data " + ex.Message);
            }
        });
    }

    public void SaveGame(GameData gameData)
    {
        service.SetProfile(gameData);
    }
}