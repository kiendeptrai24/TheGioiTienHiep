using System;
using UnityEngine;

public class SaveProfileClient : ISaveGameData<GameData, PlayerClientDataDto>
{
    public void SaveGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (gameData == null)
            {
                Debug.LogError("SetProfile failed: gameData is null");
                return;
            }
            Vector3DTO posDTO = new Vector3DTO(gameData.position);

            Vector3 rot = new Vector3(gameData.rotation.eulerAngles.x, gameData.rotation.eulerAngles.x, gameData.rotation.eulerAngles.x);

            Vector3DTO rotDTO = new Vector3DTO(rot);

            ItemDataPoint itemDataPoint = gameData.itemDataPoint;

            if (itemDataPoint == null)
            {
                itemDataPoint = new ItemDataPoint();
            }
            PlayerProfileDTO profile = new PlayerProfileDTO
            {
                characterId = gameData.characterId,
                coins = gameData.coins,
                currentHealth = gameData.currentHealth,
                playerName = gameData.characterName,
                createdAt = gameData.createdAt,
                position = posDTO,
                rotation = rotDTO,
                potentialPoint = gameData.potentialPoint,
                skillPoint = gameData.skillPoint,
                // ===== OFFLINE MINING SAVE =====
                mineOfflineDataList = gameData.mineOfflineDataList ?? new MineOfflineDataList()
            };
            playerClientDataDto.profileRes = profile;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error " + ex.Message);
        }
    }
}
