
using System;
using UnityEngine;

public class PlayerUsedItemInventoryService :  ILoadRemote<GameData>, ISaveRemote<GameData>
{
    private PlayFabDataClientService service;
    public PlayerUsedItemInventoryService(PlayFabDataClientService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadPlayerDatasUsed(gameData.characterId, (gameDataDTO) =>
        {
            try
            {
                if (gameDataDTO == null)
                {
                    callback?.Invoke();
                    return;
                }
                var itemsData = new ItemDataDTO();
                itemsData = gameDataDTO;

                var dataManager = GameDataCenterManager.Instance;

                for (int i = 0; i < itemsData.inventoryItems.Count; i++)
                {
                    var item = itemsData.inventoryItems[i];
                    if (item == null)
                    {
                        Debug.Log("item is null");
                        return;
                    }
                    var itemData = dataManager.GetItemById(item.instanceId);
                    itemsData.inventoryItems[i] = itemData;
                }
                gameData.itemUsedDatas.AddRange(itemsData.inventoryItems);
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadPlayerDatasUsed Error occurred while loading item data." + ex.Message);
            }
        });
    }

    public void SaveGame(GameData gameData)
    {
        service.SetItemInventoryDataUsed(gameData);
    }
}