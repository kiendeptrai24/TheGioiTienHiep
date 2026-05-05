using System;
using UnityEngine;

public class SaveProfileClient : ISaveGameData<GameData, PlayerClientDataDto>
{
    public void SaveGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (playerClientDataDto == null)
            {
                gameData.potentialPoint = 5;
                gameData.skillPoint = 3;
                return;
            }
            if (string.IsNullOrEmpty(playerClientDataDto.profileRes.playerName) == false)
                gameData.characterName = playerClientDataDto.profileRes.playerName;
            if (string.IsNullOrEmpty(playerClientDataDto.profileRes.characterId) == false)
                gameData.characterId = playerClientDataDto.profileRes.characterId;
            gameData.coins = playerClientDataDto.profileRes.coins;


            gameData.position = playerClientDataDto.profileRes.position.ToVector3();
            gameData.rotation = Quaternion.LookRotation(playerClientDataDto.profileRes.rotation.ToVector3());
            gameData.potentialPoint = playerClientDataDto.profileRes.potentialPoint;
            gameData.skillPoint = playerClientDataDto.profileRes.skillPoint;
            gameData.itemDataPoint = playerClientDataDto.profileRes.itemDataPoint;
            if (playerClientDataDto.profileRes.itemDataPoint == null)
            {
                gameData.itemDataPoint = new ItemDataPoint();
            }
            if (playerClientDataDto.profileRes.position.Equals(default(Vector3DTO)))
            {
                gameData.position = new Vector3(500, 0, 440);
            }
            if (playerClientDataDto.profileRes.rotation.Equals(default(Vector3DTO)))
            {
                gameData.rotation = Quaternion.identity;
            }

            // ===== LOAD OFFLINE MINING DATA =====
            if (playerClientDataDto.profileRes.mineOfflineDataList != null && playerClientDataDto.profileRes.mineOfflineDataList.Count > 0)
            {
                gameData.mineOfflineDataList = playerClientDataDto.profileRes.mineOfflineDataList;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load profile data " + ex.Message);
        }
    }
}
