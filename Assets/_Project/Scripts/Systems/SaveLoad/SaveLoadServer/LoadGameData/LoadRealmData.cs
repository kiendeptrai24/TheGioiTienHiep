

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadRealmData : ILoadGameData
{

    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<ItemRealmDataDto> realmItem = allGameDataDto.realmRes;
            if (realmItem == null)
            {
                Debug.Log("LoadGame: itemsShop is null");
                return;
            }

            List<RealmData> itemDatas = new();
            for (int i = 0; i < realmItem.Count; i++)
            {
                var itemDto = realmItem[i];
                RealmData itemData = new RealmData();
                itemData.instanceId = itemDto.instanceId;
                itemData.realmId = itemDto.instanceId;
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
                itemData.item = itemDto.item == null ? "" : itemDto.item;
                itemData.rate = DataParseUtils.ParsePercent(itemDto.rate);
                itemData.increaseRate = DataParseUtils.ParsePercent(itemDto.increaseRate);
                itemData.timeSeconds = DataParseUtils.ParseTimeToSeconds(itemDto.time);
                itemDatas.Add(itemData);
            }
            gameData.realmItems = itemDatas;
            gameData.allItems.AddRange(itemDatas);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
        }   
    }

}