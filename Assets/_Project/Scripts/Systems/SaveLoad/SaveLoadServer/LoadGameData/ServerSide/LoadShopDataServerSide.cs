using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadShopDataServerSide : ILoadGameData<GameDataCenter, AllGameDataResponseDto>
{

    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<ShopDataDto> shopItems = allGameDataDto.shopRes;
            if (shopItems == null)
            {
                Debug.Log("LoadGame: shopRes is null");
                return;
            }

            for (int i = 0; i < shopItems.Count; i++)
            {
                var shopData = DataMapper.MapShopData(shopItems[i]);
                if (shopData == null)
                {
                    Debug.LogWarning($"LoadGame: Failed to map shop data for instanceId {shopItems[i].instanceId}");
                    continue;
                }
                gameData.shopItems.Add(shopData);
                gameData.allItems.Add(shopData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load shop data " + ex.Message);
        }
    }

}