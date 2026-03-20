
using System;
using UnityEngine;

public class ShopService : ISaveLoadRemote
{
    private PlayFabDataService service;
    public ShopService(PlayFabDataService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadShopData((gameDataDTO) =>
        {
            var itemsShop = new ItemDataDTO();
            itemsShop = gameDataDTO;

            var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
            var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
            var SODataBase = ScriptableObjectLoader.Instance;

            for (int i = 0; i < itemsShop.inventoryItems.Count; i++)
            {
                var item = itemsShop.inventoryItems[i];
                var itemData = SODataBase.GetItem(item.itemId);
                if (itemData == null)
                    continue;
                var sprite = iconLoader.Get(item.itemIconPath);
                if (sprite != null)
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
            gameData.itemShopDatas = itemsShop.inventoryItems;
            callback?.Invoke();
        });
    }

    private void SetHeroData(ItemDataDTO itemsShop, IconLoader iconLoader, PrefabLoader prefabLoader, ScriptableObjectLoader SODataBase, int i, ItemData itemData, HeroData heroData)
    {
        var heroPrefab = prefabLoader.Get(itemData.itemFilePath);
        heroData.heroPrefab = heroPrefab;

        for (int h = 0; h < heroData.skillDatas.Count; h++)
        {
            var skill = heroData.skillDatas[h];

            var skillData = SODataBase.GetItem(skill.itemId) as SkillData;
            if (skillData == null)
                continue;
            skillData.itemIcon = iconLoader.Get(skillData.itemIconPath);
            skillData.skillEffectPrefab = prefabLoader.Get(skillData.itemFilePath);
            heroData.skillDatas[h] = skillData;
        }

        for (int s = 0; s < heroData.techniqueDatas.Count; s++)
        {
            var technique = heroData.techniqueDatas[s];
            var techniqueData = SODataBase.GetItem(technique.itemId) as TechniqueData;
            if (techniqueData == null)
                continue;
            heroData.techniqueDatas[s] = techniqueData;
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