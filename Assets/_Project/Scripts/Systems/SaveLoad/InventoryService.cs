
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

                var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
                var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
                var SODataBase = ScriptableObjectLoader.Instance;
                for (int i = 0; i < itemsShop.inventoryItems.Count; i++)
                {
                    var item = itemsShop.inventoryItems[i];
                    var itemData = SODataBase.GetItem(item.instanceId);
                    if (itemData == null)
                        continue;
                    itemData.itemName = gameDataDTO.inventoryItems[i].itemName;
                    itemData.realmType = gameDataDTO.inventoryItems[i].realmType;
                    var sprite = iconLoader.Get(item.itemIconPath);
                    itemData.itemIcon = sprite;

                    if (itemData is HeroData heroData)
                    {
                        SetHeroData(itemsShop, iconLoader, prefabLoader, SODataBase, i, itemData, heroData);
                        continue;
                    }

                    if (itemData is SkillData skillDatas)
                    {
                        SetSkilldata(itemsShop, iconLoader, prefabLoader, i, skillDatas);
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

    private void SetHeroData(ItemDataDTO itemsShop, IconLoader iconLoader, PrefabLoader prefabLoader, ScriptableObjectLoader SODataBase, int i, ItemData itemData, HeroData heroData)
    {
        var heroPrefab = prefabLoader.Get(itemData.itemFilePath);
        heroData.heroPrefab = heroPrefab;

        for (int h = 0; h < heroData.skillDatas.Count; h++)
        {
            var skill = heroData.skillDatas[h];

            var skillData = SODataBase.GetItem(skill.instanceId) as SkillData;
            if (skillData == null)
                continue;
            skillData.itemIcon = iconLoader.Get(skillData.itemIconPath);
            skillData.skillEffectPrefab = prefabLoader.Get(skillData.itemFilePath);
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
            equipmentData.itemIcon = iconLoader.Get(equipmentData.itemIconPath);
            heroData.equipmentDatas[k] = equipmentData;
        }
        itemsShop.inventoryItems[i] = heroData;
    }

    private void SetSkilldata(ItemDataDTO itemsShop, IconLoader iconLoader, PrefabLoader prefabLoader, int i, SkillData skillDatas)
    {
        skillDatas.itemIcon = iconLoader.Get(skillDatas.itemIconPath);
        skillDatas.skillEffectPrefab = prefabLoader.Get(skillDatas.itemFilePath);

        itemsShop.inventoryItems[i] = skillDatas;
    }
    public void SaveGame(GameData gameData)
    {

    }
}