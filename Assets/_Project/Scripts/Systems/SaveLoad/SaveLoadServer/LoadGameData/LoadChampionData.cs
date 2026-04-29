

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadChampionData : ILoadGameData
{

    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<ChampionDataDto> championResponse = allGameDataDto.championRes;
            if (championResponse == null)
            {
                Debug.Log("LoadGame: itemsShop is null");
                return;
            }

            List<HeroData> heroDatas = new();
            for (int i = 0; i < championResponse.Count; i++)
            {
                HeroData heroData = new HeroData();
                var itemDto = championResponse[i];
                heroData.instanceId = itemDto.instanceId;
                heroData.itemName = itemDto.name;
                heroData.itemDescription = itemDto.description;
                heroData.qualityType = itemDto.quality;
                heroData.raceId = itemDto.raceId;
                heroData.realmId = itemDto.realmId;
                heroData.essenceId = itemDto.essenceId;
                heroData.elementType = itemDto.elementType;
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

                heroData.techniqueIds.Add(itemDto.techniqueId);
                heroData.skillIds = itemDto.skillsId;
                heroDatas.Add(heroData);
            }
            gameData.championItems = heroDatas;
            gameData.allItems.AddRange(heroDatas);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
        }
    }

}