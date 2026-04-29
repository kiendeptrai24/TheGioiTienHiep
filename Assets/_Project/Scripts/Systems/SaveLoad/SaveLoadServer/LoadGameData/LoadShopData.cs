

using System;
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

            List<ItemShop> itemDatas = new();
            for (int i = 0; i < shopResponse.Count; i++)
            {
                ItemShop itemData = new ItemShop();
                var itemDto = shopResponse[i];
                itemData.instanceId = itemDto.instanceId;
                itemData.price = itemDto.price;
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