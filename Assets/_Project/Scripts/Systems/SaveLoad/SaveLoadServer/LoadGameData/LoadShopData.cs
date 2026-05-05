using System.Collections.Generic;
using UnityEngine;

public class LoadShopData : ILoadGameData
{

    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<ShopDataDto> shopResponse = allGameDataDto.shopRes;
            if (shopResponse == null)
            {
                Debug.Log("LoadGame: itemsShop is null");
                return;
            }

            List<ItemData> itemDatas = new();
            for (int i = 0; i < shopResponse.Count; i++)
            {
                ItemData itemData = new ItemData();
                var itemDto = shopResponse[i];
                itemData.instanceId = itemDto.instanceId;
                itemData.itemPrice = itemDto.price;
                itemDatas.Add(itemData);
            }
            gameData.shopItems.AddRange(itemDatas);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
        }
    }

}