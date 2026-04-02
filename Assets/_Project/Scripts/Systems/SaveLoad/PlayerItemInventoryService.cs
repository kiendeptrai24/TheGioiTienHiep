
using System;
using UnityEngine;

public class PlayerItemInventoryService : ISaveLoadRemote
{
    private PlayFabDataService service;
    public PlayerItemInventoryService(PlayFabDataService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadPlayerData(gameData.characterId, (gameDataDTO) =>
        {
            if (gameDataDTO == null)
            {
                callback?.Invoke();
                return;
            }
            var itemsData = new ItemDataDTO();
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
                var sprite = iconLoader.Get(item.itemIconPath);
                itemData.itemIcon = sprite;

                if (itemData is HeroData heroData)
                {
                    SetHeroData(itemsData, iconLoader, prefabLoader, SODataBase, i, itemData, heroData);
                    if (heroData.isCharactor)
                    {
                        if (gameData.itemDataPoint != null)
                        {
                            heroData.physicalDamagePoint = gameData.itemDataPoint.damagePoint;
                            heroData.magicalDamagePoint = gameData.itemDataPoint.damagePoint;
                            heroData.spiritDamagePoint = gameData.itemDataPoint.damagePoint;
                            heroData.physicalDefensePoint = gameData.itemDataPoint.defensePoint;
                            heroData.magicalDefensePoint = gameData.itemDataPoint.defensePoint;
                            heroData.spiritDefensePoint = gameData.itemDataPoint.defensePoint;
                            heroData.healthPoint = gameData.itemDataPoint.healthPoint;
                            heroData.manaPoint = gameData.itemDataPoint.manaPoint;
                            heroData.spiritPoint = gameData.itemDataPoint.spiritPoint;
                            heroData.moveSpeedPoint = gameData.itemDataPoint.moveSpeed;
                            heroData.spititRangePoint = gameData.itemDataPoint.spititRange;
                        }
                    }
                    continue;
                }

                if (itemData is SkillData skillDatas)
                {
                    SetSkilldata(itemsData, iconLoader, prefabLoader, i, skillDatas);
                    continue;
                }
                itemsData.inventoryItems[i] = itemData;
            }
            gameData.itemDatas = itemsData.inventoryItems;
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
        service.SetItemInventoryData(gameData);
    }
}