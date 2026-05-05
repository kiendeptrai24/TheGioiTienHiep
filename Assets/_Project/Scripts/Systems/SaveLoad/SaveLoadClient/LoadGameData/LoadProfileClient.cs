using System;
using UnityEngine;

public class LoadProfileClient : ILoadGameData<GameData, PlayerClientDataDto>
{
    public void LoadGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (playerClientDataDto == null || playerClientDataDto.profileRes == null)
            {
                Debug.LogWarning("LoadProfileClient: profileRes is null");
                return;
            }

            var profile = playerClientDataDto.profileRes;
            if (string.IsNullOrEmpty(profile.playerName) == false)
                gameData.characterName = profile.playerName;
            if (string.IsNullOrEmpty(profile.characterId) == false)
                gameData.characterId = profile.characterId;
            gameData.coins = profile.coins;
            gameData.position = profile.position.ToVector3();
            gameData.rotation = Quaternion.LookRotation(profile.rotation.ToVector3());
            gameData.potentialPoint = profile.potentialPoint;
            gameData.skillPoint = profile.skillPoint;
            gameData.itemDataPoint = profile.itemDataPoint;
            if (profile.itemDataPoint == null)
            {
                gameData.itemDataPoint = new ItemDataPoint();
            }
            if (profile.position.Equals(default(Vector3DTO)))
            {
                gameData.position = new Vector3(500, 0, 440);
            }
            if (profile.rotation.Equals(default(Vector3DTO)))
            {
                gameData.rotation = Quaternion.identity;
            }
            // Load offline mining data if available
            if (profile.mineOfflineDataList != null && profile.mineOfflineDataList.Count > 0)
            {
                gameData.mineOfflineDataList = profile.mineOfflineDataList;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadProfileClient failed: {ex.Message}");
        }
    }
}