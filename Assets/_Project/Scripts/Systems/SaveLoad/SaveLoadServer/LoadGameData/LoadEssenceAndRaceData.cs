

using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadEssenceAndRaceData : ILoadGameData
{

    public void LoadGameData(GameDataCenter gameData, AllGameDataResponseDto allGameDataDto)
    {
        try
        {
            List<EssenceAndRaceDataDto> realmItem = allGameDataDto.essenceAndRaceRes;
            if (realmItem == null)
            {
                Debug.Log("LoadGame: itemsShop is null");
                return;
            }

            List<ItemData> itemDatas = new();
            List<EssenceData> essenceDatas = new();
            List<RaceData> raceDatas = new();
            for (int i = 0; i < realmItem.Count; i++)
            {
                var itemDto = realmItem[i];
                ItemData itemData = null;
                if (itemDto.type == EssenceAndRaceType.Essence)
                {
                    var essenceData = new EssenceData();
                    if (itemDto.essenceType.HasValue)
                        essenceData.essenceType = itemDto.essenceType.Value;
                    essenceData.instanceId = itemDto.instanceId;
                    essenceData.itemName = itemDto.instanceId;
                    essenceData.itemDescription = itemDto.instanceId;
                    essenceData.physicalDamagePoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.physicalDamagePoint);
                    essenceData.magicalDamagePoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.magicalDamagePoint);
                    essenceData.spiritDamagePoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.spiritDamagePoint);
                    essenceData.physicalDefensePoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.physicalDefensePoint);
                    essenceData.magicalDefensePoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.magicalDefensePoint);
                    essenceData.spiritDefensePoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.spiritDefensePoint);
                    essenceData.healthPoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.healthPoint);
                    essenceData.manaPoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.manaPoint);
                    essenceData.spiritPoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.spiritPoint);
                    essenceData.spiritRangePoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.spiritRangePoint);
                    essenceData.movementSpeedPoint = (int)DataParseUtils.ParseNumberOrPercent(itemDto.movementSpeedPoint);
                    itemData = essenceData;
                    essenceDatas.Add(essenceData);
                }
                else if (itemDto.type == EssenceAndRaceType.Race)
                {
                    var raceData = new RaceData();
                    if (itemDto.raceType.HasValue)
                        raceData.raceType = itemDto.raceType.Value;
                    raceData.instanceId = itemDto.instanceId;
                    raceData.itemName = itemDto.instanceId;
                    raceData.healthPoint = DataParseUtils.ParseNumberOrPercent(itemDto.healthPoint);
                    raceData.manaPoint = DataParseUtils.ParseNumberOrPercent(itemDto.manaPoint);
                    raceData.spiritPoint = DataParseUtils.ParseNumberOrPercent(itemDto.spiritPoint);
                    raceData.physicalDamagePoint = DataParseUtils.ParseNumberOrPercent(itemDto.physicalDamagePoint);
                    raceData.magicalDamagePoint = DataParseUtils.ParseNumberOrPercent(itemDto.magicalDamagePoint);
                    raceData.spiritDamagePoint = DataParseUtils.ParseNumberOrPercent(itemDto.spiritDamagePoint);
                    raceData.physicalDefensePoint = DataParseUtils.ParseNumberOrPercent(itemDto.physicalDefensePoint);
                    raceData.spiritDefensePoint = DataParseUtils.ParseNumberOrPercent(itemDto.spiritDefensePoint);
                    raceData.magicalDefensePoint = DataParseUtils.ParseNumberOrPercent(itemDto.magicalDefensePoint);
                    raceData.spiritRangePoint = DataParseUtils.ParseNumberOrPercent(itemDto.spiritRangePoint);
                    raceData.movementSpeedPoint = DataParseUtils.ParseNumberOrPercent(itemDto.movementSpeedPoint);
                    itemData = raceData;
                    raceDatas.Add(raceData);
                }

                itemDatas.Add(itemData);
            }
            gameData.raceItems = raceDatas;
            gameData.essenceItems = essenceDatas;
            gameData.allItems.AddRange(itemDatas);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
        }
    }

}