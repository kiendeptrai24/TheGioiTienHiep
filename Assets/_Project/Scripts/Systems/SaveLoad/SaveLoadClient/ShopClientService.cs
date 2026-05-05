
using System;
using UnityEngine;

public class ShopClientService : ISaveLoadRemote
{
    private PlayFabDataClientService service;
    public ShopClientService(PlayFabDataClientService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadShopData((gameDataDTO) =>
        {
            try
            {
                var itemsShop = new ItemDataDTO();
                itemsShop = gameDataDTO;

                var dataManager = GameDataCenterManager.Instance;

                for (int i = 0; i < itemsShop.inventoryItems.Count; i++)
                {
                    var item = itemsShop.inventoryItems[i];
                    var itemData = dataManager.GetItemById(item.instanceId);
                    if (itemData == null)
                        continue;

                    if (itemData is HeroData heroData)
                    {
                        SetHeroData(itemsShop, dataManager, i, heroData);
                        continue;
                    }

                    itemsShop.inventoryItems[i] = itemData;
                }
                gameData.itemShopDatas = itemsShop.inventoryItems;
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadGame: Failed to load shop data " + ex.Message);
            }
        });
    }

    private void SetHeroData(ItemDataDTO itemsShop, GameDataCenterManager dataManager, int i, HeroData heroData)
    {

        for (int h = 0; h < heroData.skillDatas.Count; h++)
        {
            var skill = heroData.skillDatas[h];

            var skillData = dataManager.GetItemById(skill.instanceId) as SkillData;
            if (skillData == null)
                continue;
            heroData.skillDatas[h] = skillData;
        }

        for (int s = 0; s < heroData.techniqueDatas.Count; s++)
        {
            var technique = heroData.techniqueDatas[s];
            var techniqueData = dataManager.GetItemById(technique.instanceId) as TechniqueData;
            if (techniqueData == null)
                continue;
            heroData.techniqueDatas[s] = techniqueData;
        }

        itemsShop.inventoryItems[i] = heroData;
    }
    public void SaveGame(GameData gameData)
    {

    }
}