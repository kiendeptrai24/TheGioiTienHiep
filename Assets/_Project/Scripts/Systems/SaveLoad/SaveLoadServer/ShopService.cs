
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopService : ILoadRemoteServer
{
    private PlayFabDataServerService service;
    public ShopService(PlayFabDataServerService service)
    {
        this.service = service;
    }

    public void LoadGame(GameDataServer gameData, Action callback)
    {
        service.LoadShopData((gameDataDTO) =>
        {
            try
            {
                ShopResponseDto shopResponse = gameDataDTO;
                if (shopResponse == null)
                {
                    Debug.Log("LoadGame: itemsShop is null");
                    return;
                }

                List<ItemShop> itemDatas = new();
                for (int i = 0; i < shopResponse.Data.Count; i++)
                {
                    ItemShop itemData = new ItemShop();
                    var itemDto = shopResponse.Data[i];
                    itemData.instanceId = itemDto.instanceId;
                    itemData.price = itemDto.price;
                    itemDatas.Add(itemData);
                }
                gameData.shopItems.AddRange(itemDatas);
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
            }
        });
    }
}