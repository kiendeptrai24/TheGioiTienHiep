
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RealmService : ILoadRemoteServer
{
    private PlayFabDataServerService service;
    public RealmService(PlayFabDataServerService service)
    {
        this.service = service;
    }

    public void LoadGame(GameDataServer gameData, Action callback)
    {
        service.LoadRealmData((gameDataDTO) =>
        {
            try
            {
                RealmResponseDto realmItem = gameDataDTO;
                if (realmItem == null)
                {
                    Debug.Log("LoadGame: itemsShop is null");
                    return;
                }

                List<ItemData> itemDatas = new();
                for (int i = 0; i < realmItem.Data.Count; i++)
                {
                    var itemDto = realmItem.Data[i];
                    RealmData itemData = new RealmData();
                    itemData.instanceId = itemDto.instanceId;
                    itemData.realmType = itemDto.realmType;
                    itemData.maxHealth = itemDto.health;
                    itemData.maxMana = itemDto.mana;
                    itemData.maxSpirit = itemDto.spirit;
                    itemData.physicalDamage = itemDto.physicalDamage;
                    itemData.magicalDamage = itemDto.magicalDamage;
                    itemData.spiritDamage = itemDto.spiritDamage;
                    itemData.physicalDefense = itemDto.physicalDefense;
                    itemData.magicalDefense = itemDto.magicalDefense;
                    itemData.spiritDefense = itemDto.spiritDefense;
                    itemData.spiritRange = itemDto.spiritCritRate;
                    itemData.movementSpeed = itemDto.movementSpeed;
                    itemData.rewardPotentialPoint = itemDto.potentialPoints;
                    itemData.rewardSkillPoint = itemDto.skillPoints;
                    itemData.lthao = itemDto.lthao;
                    itemData.item = itemDto.item;
                    itemData.rate = DataParseUtils.ParsePercent(itemDto.rate);
                    itemData.increaseRate = DataParseUtils.ParsePercent(itemDto.increaseRate);
                    itemData.timeSeconds = DataParseUtils.ParseTimeToSeconds(itemDto.time);
                    itemDatas.Add(itemData);
                }
                gameData.realmItems = itemDatas;
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