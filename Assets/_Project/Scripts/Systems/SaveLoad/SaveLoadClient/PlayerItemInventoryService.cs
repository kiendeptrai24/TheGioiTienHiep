
using System;
using UnityEngine;

public class PlayerItemInventoryService : ISaveLoadRemote
{
    private PlayFabDataClientService service;
    public PlayerItemInventoryService(PlayFabDataClientService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadPlayerData(gameData.characterId, (gameDataDTO) =>
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

                var SODataBase = ScriptableObjectLoader.Instance;

                for (int i = 0; i < itemsData.inventoryItems.Count; i++)
                {
                    var item = itemsData.inventoryItems[i];
                    if (item == null)
                    {
                        Debug.Log("item is null");
                        return;
                    }
                    var itemData = SODataBase.GetItem(item.instanceId);
                    itemData.itemName = gameDataDTO.inventoryItems[i].itemName;
                    itemData.realmType = gameDataDTO.inventoryItems[i].realmType;

                    if (itemData is HeroData)
                        continue;

                    itemsData.inventoryItems[i] = itemData;
                }
                gameData.itemDatas.AddRange(itemsData.inventoryItems);
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error occurred while loading item data." + ex.Message);
            }
        });
    }
    public void SaveGame(GameData gameData)
    {
        service.SetItemInventoryData(gameData);
    }
}