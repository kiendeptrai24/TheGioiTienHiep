

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadEquipmentData : ILoadGameData
{
    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameData)
    {
        try
        {
            List<ItemDataDto> allItem = allGameData.equipmentRes;
            if (allItem == null)
            {
                Debug.Log("LoadGame: itemsShop is null");
                return;
            }

            var SODataBase = ScriptableObjectLoader.Instance;
            List<ItemData> itemDatas = new();
            List<EquitmentData> equipmentDatas = new();
            List<TechniqueData> techniqueDatas = new();
            List<SkillData> skillDatas = new();
            for (int i = 0; i < allItem.Count; i++)
            {
                var itemDto = allItem[i];
                var itemBase = SODataBase.GetItem(itemDto.itemInstanceId);
                ItemData itemData = null;
                itemData = CreateItem(itemDto, equipmentDatas, skillDatas, techniqueDatas);
                itemData.instanceId = itemDto.itemInstanceId;
                itemData.itemName = itemDto.itemName;
                itemData.itemDescription = itemDto.description;
                itemData.itemType = itemDto.itemType;
                //itemData.itemIcon = itemBase.itemIcon;
                itemData.realmType = itemDto.realmType;
                itemData.qualityType = itemDto.qualityType;
                itemData.physicalDamage = itemDto.physicalDamage;
                itemData.magicalDamage = itemDto.magicalDamage;
                itemData.spiritDamage = itemDto.spiritDamage;
                itemData.physicalDefense = itemDto.physicalDefense;
                itemData.magicalDefense = itemDto.magicalDefense;
                itemData.spiritDefense = itemDto.sppiritDefense;
                itemData.potentialPoints = itemDto.potentialPoints;

                itemDatas.Add(itemData);
            }
            gameData.equipmentItems = equipmentDatas;
            gameData.skillItems = skillDatas;
            gameData.techniqueDatasItems = techniqueDatas;
            gameData.allItems.AddRange(itemDatas);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
        }
    }
    private static ItemData CreateItem(ItemDataDto itemDto, List<EquitmentData> equipmentDatas, List<SkillData> skillDatas, List<TechniqueData> techniqueDatas)
    {
        ItemData itemData;
        if (itemDto.itemType == ItemType.Equipment)
        {
            var equipData = new EquitmentData();
            equipData.raceType = itemDto.raceType;
            if (itemDto.equipmentType.HasValue)
                equipData.equipmentType = itemDto.equipmentType.Value;
            itemData = equipData;
            equipmentDatas.Add(equipData);
        }
        else if (itemDto.itemType == ItemType.Skill)
        {
            var skillData = new SkillData();
            skillData.raceType = itemDto.raceType;
            if (itemDto.skillType.HasValue)
                skillData.skillType = itemDto.skillType.Value;
            itemData = skillData;
            skillDatas.Add(skillData);
        }
        else if (itemDto.itemType == ItemType.Technique)
        {
            var techniqueData = new TechniqueData();
            techniqueData.raceType = itemDto.raceType;
            if (itemDto.techniqueType.HasValue)
                techniqueData.techniqueType = itemDto.techniqueType.Value;
            itemData = techniqueData;
            techniqueDatas.Add(techniqueData);
        }
        else
        {
            itemData = new ItemData();
        }

        return itemData;
    }

}