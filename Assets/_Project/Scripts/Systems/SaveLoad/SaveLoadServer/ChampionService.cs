
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionService : ILoadRemoteServer
{
    private PlayFabDataServerService service;
    public ChampionService(PlayFabDataServerService service)
    {
        this.service = service;
    }

    public void LoadGame(GameDataServer gameData, Action callback)
    {
        service.LoadChampionData((gameDataDTO) =>
        {
            try
            {
                ChampionResponseDto championResponse = gameDataDTO;
                if (championResponse == null)
                {
                    Debug.Log("LoadGame: itemsShop is null");
                    return;
                }

                List<ItemData> itemDatas = new();
                for (int i = 0; i < championResponse.Data.Count; i++)
                {
                    HeroData heroData = new HeroData();
                    var itemDto = championResponse.Data[i];
                    heroData.instanceId = itemDto.instanceId;
                    heroData.itemName = itemDto.name;
                    heroData.itemDescription = itemDto.description;
                    heroData.qualityType = itemDto.quality;
                    heroData.essenceType = itemDto.essenceType;
                    heroData.raceType = itemDto.raceType;
                    heroData.elementType = itemDto.elementType;
                    heroData.realmType = itemDto.realmType;
                    heroData.attackRange = itemDto.attackRange;

                    heroData.healthPoint = itemDto.healthPoint;
                    heroData.manaPoint = itemDto.manaPoint;
                    heroData.spiritPoint = itemDto.spiritPoint;

                    heroData.physicalDamagePoint = itemDto.physicalDamagePoint;
                    heroData.magicalDamagePoint = itemDto.magicalDamagePoint;
                    heroData.spiritDamagePoint = itemDto.spiritDamagePoint;

                    heroData.physicalDefensePoint = itemDto.physicalDefensePoint;
                    heroData.magicalDefensePoint = itemDto.magicalDefensePoint;
                    heroData.spiritDefensePoint = itemDto.spiritDefensePoint;

                    heroData.healthBonus = DataParseUtils.ParseNumberOrPercent(itemDto.healthBonus);
                    heroData.manaBonus = DataParseUtils.ParseNumberOrPercent(itemDto.manaBonus);
                    heroData.spiritBonus = DataParseUtils.ParseNumberOrPercent(itemDto.spiritBonus);

                    heroData.physicalDamageBonus = DataParseUtils.ParseNumberOrPercent(itemDto.physicalDamageBonus);
                    heroData.magicalDamageBonus = DataParseUtils.ParseNumberOrPercent(itemDto.magicalDamageBonus);
                    heroData.spiritDamageBonus = DataParseUtils.ParseNumberOrPercent(itemDto.spiritDamageBonus);

                    heroData.physicalDefenseBonus = DataParseUtils.ParseNumberOrPercent(itemDto.physicalDefenseBonus);
                    heroData.magicalDamageBonus = DataParseUtils.ParseNumberOrPercent(itemDto.magicalDefenseBonus);
                    heroData.spiritDefenseBonus = DataParseUtils.ParseNumberOrPercent(itemDto.spiritDefenseBonus);
                    itemDatas.Add(heroData);
                }
                gameData.championItems = itemDatas;
                gameData.allItems.AddRange(itemDatas);
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
            }
        });
    }
    private static ItemData CreateItem(ItemDataDto itemDto)
    {
        ItemData itemData;
        if (itemDto.itemType == ItemType.Equipment)
        {
            var equipData = new EquitmentData();
            equipData.raceType = itemDto.raceType;
            if (itemDto.equipmentType.HasValue)
                equipData.equipmentType = itemDto.equipmentType.Value;
            itemData = equipData;
        }
        else if (itemDto.itemType == ItemType.Skill)
        {
            var skillData = new SkillData();
            skillData.raceType = itemDto.raceType;
            if (itemDto.skillType.HasValue)
                skillData.skillType = itemDto.skillType.Value;
            itemData = skillData;
        }
        else if (itemDto.itemType == ItemType.Technique)
        {
            var techniqueData = new TechniqueData();
            techniqueData.raceType = itemDto.raceType;
            if (itemDto.techniqueType.HasValue)
                techniqueData.techniqueType = itemDto.techniqueType.Value;
            itemData = techniqueData;
        }
        else
        {
            itemData = new ItemData();
        }

        return itemData;
    }

    public void SaveGame(GameData gameData)
    {

    }
}