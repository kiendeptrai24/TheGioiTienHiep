using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
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
        var gameData = GameDataCenterManager.Instance;

        for (int i = 0; i < heroDTO.inventoryItems.Count; i++)
        {
            var item = heroDTO.inventoryItems[i];
            var itemData = gameData.GetItemById(item.instanceId);
            itemData.itemName = heroDTO.inventoryItems[i].itemName;
            if (itemData == null)
                continue;
            var heroData = itemData as HeroData;
            if (heroData != null)
            {
                heroData.championIndex = heroDTO.championsIndex[i];
                SetHeroData(heroDTO, gameData, i, heroData);
            }
            else
                continue;

            heroDTO.inventoryItems[i] = heroData;
        }
        itemDatas = heroDTO.inventoryItems.ToList<ItemData>();
        return itemDatas;
    }

    private static void SetHeroData(HeroInTeamDataDTO itemsteam, GameDataCenterManager gameData, int i, HeroData heroData)
    {
        try
        {
            heroData.skillDatas.Clear();
            heroData.techniqueDatas.Clear();
            heroData.equipmentDatas.Clear();

            var champion = itemsteam.inventoryItems[i];

            if (champion == null) return;
            var skillDatas = champion.skillDatas;

            for (int h = 0; h < skillDatas.Count; h++)
            {
                var skill = skillDatas[h];

                var skillData = gameData.GetItemById(skill.instanceId) as SkillData;
                if (skillData == null)
                    continue;
                heroData.skillDatas.Add(skillData);
            }

            var techniqueDatas = champion.techniqueDatas;
            for (int s = 0; s < techniqueDatas.Count; s++)
            {
                var technique = techniqueDatas[s];
                var techniqueData = gameData.GetItemById(technique.instanceId) as TechniqueData;
                if (techniqueData == null)
                    continue;
                heroData.techniqueDatas.Add(techniqueData);
            }
            var equipmentDatas = champion.equipmentDatas;
            for (int k = 0; k < equipmentDatas.Count; k++)
            {
                var equipment = equipmentDatas[k];
                var equipmentData = gameData.GetItemById(equipment.instanceId) as EquipmentData;
                if (equipmentData == null)
                    continue;
                heroData.equipmentDatas.Add(equipmentData);
            }
            var raceData = gameData.GetItemById(heroData.raceId) as RaceData;
            if (raceData != null)
                heroData.raceData = raceData;
            var essenceData = gameData.GetItemById(heroData.essenceId) as EssenceData;
            if (essenceData != null)
                heroData.essenceData = essenceData;
            var realmData = gameData.GetItemById(heroData.realmId) as RealmData;
            if (realmData != null)
                heroData.realmData = realmData;
            itemsteam.inventoryItems[i] = heroData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SetHeroData: Failed to set hero data " + ex.Message);
        }
    }
}