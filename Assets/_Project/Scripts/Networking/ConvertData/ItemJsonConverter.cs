using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ItemJsonConverter
{
    public static List<ItemData> FromJson(string json)
    {
        HeroInTeamDataDTO itemDataDTO = JsonConvert.DeserializeObject<HeroInTeamDataDTO>(json);
        return Convert(itemDataDTO);
    }

    public static string ToJson(List<ItemData> itemDatas)
    {
        var itemDataDTO = new HeroInTeamDataDTO();
        foreach (var item in itemDatas)
        {
            itemDataDTO.inventoryItems.Add(item as HeroData);
            if (item is HeroData heroData)
            {
                itemDataDTO.championsIndex.Add(heroData.championIndex);
            }
        }
        return JsonConvert.SerializeObject(itemDataDTO);
    }
    public static List<ItemData> Convert(HeroInTeamDataDTO heroDTO)
    {
        var itemDatas = new List<ItemData>();
        var iconLoader = AddressableLoader.Instance.GetLoader<IconLoader>(AddressableLoaderType.Sprite.ToString());
        var prefabLoader = AddressableLoader.Instance.GetLoader<PrefabLoader>(AddressableLoaderType.Prefab.ToString());
        var SODataBase = ScriptableObjectLoader.Instance;

        for (int i = 0; i < heroDTO.inventoryItems.Count; i++)
        {
            var item = heroDTO.inventoryItems[i];
            var itemData = SODataBase.GetItem(item.instanceId);
            itemData.itemName = heroDTO.inventoryItems[i].itemName;
            if (itemData == null)
                continue;
            var sprite = iconLoader.Get(item.itemIconPath);
            itemData.itemIcon = sprite;
            var heroData = itemData as HeroData;
            if (heroData != null)
            {
                heroData.championIndex = heroDTO.championsIndex[i];
                SetHeroData(heroDTO, iconLoader, prefabLoader, SODataBase, i, itemData, heroData);
            }
            else
                continue;

            heroDTO.inventoryItems[i] = heroData;
        }
        itemDatas = heroDTO.inventoryItems.ToList<ItemData>();
        return itemDatas;
    }

    private static void SetHeroData(HeroInTeamDataDTO itemsteam, IconLoader iconLoader, PrefabLoader prefabLoader, ScriptableObjectLoader SODataBase, int i, ItemData itemData, HeroData heroData)
    {
        try
        {
            var heroPrefab = prefabLoader.Get(itemData.itemFilePath);
            heroData.skillDatas.Clear();
            heroData.techniqueDatas.Clear();
            heroData.equipmentDatas.Clear();
            heroData.heroPrefab = heroPrefab;

            var champion = itemsteam.inventoryItems[i];

            if (champion == null) return;
            var skillDatas = champion.skillDatas;

            for (int h = 0; h < skillDatas.Count; h++)
            {
                var skill = skillDatas[h];

                var skillData = SODataBase.GetItem(skill.instanceId) as SkillData;
                if (skillData == null)
                    continue;
                heroData.skillDatas.Add(skillData);
            }

            var techniqueDatas = champion.techniqueDatas;
            for (int s = 0; s < techniqueDatas.Count; s++)
            {
                var technique = techniqueDatas[s];
                var techniqueData = SODataBase.GetItem(technique.instanceId) as TechniqueData;
                if (techniqueData == null)
                    continue;
                heroData.techniqueDatas.Add(techniqueData);
            }
            var equipmentDatas = champion.equipmentDatas;
            for (int k = 0; k < equipmentDatas.Count; k++)
            {
                var equipment = equipmentDatas[k];
                var equipmentData = SODataBase.GetItem(equipment.instanceId) as EquitmentData;
                if (equipmentData == null)
                    continue;
                heroData.equipmentDatas.Add(equipmentData);
            }
            var statRace = SODataBase.GetRaceItem(heroData.raceType);
            if (statRace != null)
                heroData.raceData = statRace;
            itemsteam.inventoryItems[i] = heroData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SetHeroData: Failed to set hero data " + ex.Message);
        }
    }
}