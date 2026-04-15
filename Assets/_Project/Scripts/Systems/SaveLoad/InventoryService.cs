
using System;
using UnityEngine;

public class InventoryService : ISaveLoadRemote
{
    private PlayFabDataService service;
    public InventoryService(PlayFabDataService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadData((gameDataDTO) =>
        {
            try
            {
                ItemDataDTO itemsShop = gameDataDTO;
                if (itemsShop == null)
                {
                    Debug.Log("LoadGame: itemsShop is null");
                    return;
                }

                var SODataBase = ScriptableObjectLoader.Instance;
                for (int i = 0; i < itemsShop.inventoryItems.Count; i++)
                {
                    var item = itemsShop.inventoryItems[i];
                    var itemData = SODataBase.GetItem(item.instanceId);
                    if (itemData == null)
                        continue;
                    itemData.itemName = gameDataDTO.inventoryItems[i].itemName;
                    itemData.realmType = gameDataDTO.inventoryItems[i].realmType;

                    if (itemData is HeroData heroData)
                    {
                        SetHeroData(itemsShop, SODataBase, i, heroData);
                        continue;
                    }

                    itemsShop.inventoryItems[i] = itemData;
                }
                gameData.allItemsDatas = itemsShop.inventoryItems;
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
            }
        });
    }

    private void SetHeroData(ItemDataDTO itemsShop, ScriptableObjectLoader SODataBase, int i, HeroData heroData)
    {
        for (int h = 0; h < heroData.skillDatas.Count; h++)
        {
            var skill = heroData.skillDatas[h];

            var skillData = SODataBase.GetItem(skill.instanceId) as SkillData;
            if (skillData == null)
                continue;
            heroData.skillDatas[h] = skillData;
        }

        for (int s = 0; s < heroData.techniqueDatas.Count; s++)
        {
            var technique = heroData.techniqueDatas[s];
            var techniqueData = SODataBase.GetItem(technique.instanceId) as TechniqueData;
            if (techniqueData == null)
                continue;
            heroData.techniqueDatas[s] = techniqueData;
        }
        for (int k = 0; k < heroData.equipmentDatas.Count; k++)
        {
            var equipment = heroData.equipmentDatas[k];
            var equipmentData = SODataBase.GetItem(equipment.instanceId) as EquitmentData;
            if (equipmentData == null)
                continue;
            heroData.equipmentDatas[k] = equipmentData;
        }
        itemsShop.inventoryItems[i] = heroData;
    }

    public void SaveGame(GameData gameData)
    {

    }
}