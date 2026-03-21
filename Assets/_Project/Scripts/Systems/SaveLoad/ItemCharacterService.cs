
using System;
using UnityEngine;

public class ItemCharacterService : ISaveLoadRemote
{
    private PlayFabDataService service;
    public ItemCharacterService(PlayFabDataService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadCharacter((gameDataDTO) =>
        {
            if (gameDataDTO == null)
            {
                callback?.Invoke();
                return;
            }

            var itemsData = new ItemCharacterDataDTO();
            itemsData = gameDataDTO;

            var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
            var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
            var SODataBase = ScriptableObjectLoader.Instance;

            for (int i = 0; i < itemsData.inventoryItems.Count; i++)
            {
                var item = itemsData.inventoryItems[i];
                var itemData = SODataBase.GetItem(item.itemId);
                itemData.itemName = gameDataDTO.inventoryItems[i].itemName;
                if (itemData == null)
                    continue;
                itemData.itemName = itemsData.characterNames[i];
                (itemData as HeroData).characterId = itemsData.characterIds[i];
                var sprite = iconLoader.Get(item.itemIconPath);
                itemData.itemIcon = sprite;

                if (itemData is HeroData heroData)
                {
                    SetHeroData(itemsData, iconLoader, prefabLoader, SODataBase, i, itemData, heroData);
                    continue;
                }

                if (itemData is SkillData skillDatas)
                {
                    SetSkilldata(itemsData, iconLoader, prefabLoader, i, skillDatas);
                    continue;
                }

                itemsData.inventoryItems[i] = itemData;
            }
            gameData.itemDatasCharacter = itemsData.inventoryItems;
            callback?.Invoke();
        });
    }

    private static void SetHeroData(ItemDataDTO itemsData, IconLoader iconLoader, PrefabLoader prefabLoader, ScriptableObjectLoader SODataBase, int i, ItemData itemData, HeroData heroData)
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

        itemsData.inventoryItems[i] = heroData;
    }

    private void SetSkilldata(ItemDataDTO itemsData, IconLoader iconLoader, PrefabLoader prefabLoader, int i, SkillData skillDatas)
    {
        skillDatas.itemIcon = iconLoader.Get(skillDatas.itemIconPath);
        skillDatas.skillEffectPrefab = prefabLoader.Get(skillDatas.itemFilePath);

        itemsData.inventoryItems[i] = skillDatas;
    }
    public void SaveGame(GameData gameData)
    {
        service.SetItemCharacter(gameData);
    }
}