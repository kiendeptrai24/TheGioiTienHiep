
using System;
using UnityEngine;

public class TeamInventoryService : ISaveLoadRemote
{
    private PlayFabLogin playFabLogin;
    public TeamInventoryService(PlayFabLogin playFabLogin)
    {
        this.playFabLogin = playFabLogin;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        playFabLogin.player.LoadTeamData((gameDataDTO) =>
        {
            var itemTeam = new HeroDataDTO();
            itemTeam = gameDataDTO;

            var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
            var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
            var SODataBase = ScriptableObjectLoader.Instance;

            for (int i = 0; i < itemTeam.inventoryItems.Count; i++)
            {
                var item = itemTeam.inventoryItems[i];
                var itemData = SODataBase.GetItem(item.itemId);
                if (itemData == null)
                    continue;
                var sprite = iconLoader.Get(item.itemIconPath);
                itemData.itemIcon = sprite;

                if (itemData is HeroData heroData)
                {
                    heroData.championIndex = itemTeam.championsIndex[i];
                    SetHeroData(itemTeam, iconLoader, prefabLoader, SODataBase, i, itemData, heroData);
                    continue;
                }

                if (itemData is SkillData skillDatas)
                {
                    SetSkilldata(itemTeam, iconLoader, prefabLoader, i, skillDatas);
                    continue;
                }

                itemTeam.inventoryItems[i] = itemData;
            }
            gameData.itemDatasInTeam = itemTeam.inventoryItems;
            callback?.Invoke();
        });
    }

    private void SetHeroData(ItemDataDTO itemsteam, IconLoader iconLoader, PrefabLoader prefabLoader, ScriptableObjectLoader SODataBase, int i, ItemData itemData, HeroData heroData)
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

        itemsteam.inventoryItems[i] = heroData;
    }

    private void SetSkilldata(ItemDataDTO itemsteam, IconLoader iconLoader, PrefabLoader prefabLoader, int i, SkillData skillDatas)
    {
        skillDatas.itemIcon = iconLoader.Get(skillDatas.itemIconPath);
        skillDatas.skillEffectPrefab = prefabLoader.Get(skillDatas.itemFilePath);

        itemsteam.inventoryItems[i] = skillDatas;
    }
    public void SaveGame(GameData gameData)
    {
        playFabLogin.player.SetTeamData(gameData);
    }
}