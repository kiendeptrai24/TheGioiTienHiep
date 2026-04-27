
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EssenceAndRaceService : ILoadRemoteServer
{
    private PlayFabDataServerService service;
    public EssenceAndRaceService(PlayFabDataServerService service)
    {
        this.service = service;
    }

    public void LoadGame(GameDataServer gameData, Action callback)
    {
        service.LoadEssenceAndRaceData((gameDataDTO) =>
        {
            try
            {
                ItemEssenceAndRaceResponseDto realmItem = gameDataDTO;
                if (realmItem == null)
                {
                    Debug.Log("LoadGame: itemsShop is null");
                    return;
                }

                List<ItemData> itemDatas = new();
                for (int i = 0; i < realmItem.Data.Count; i++)
                {
                    var itemDto = realmItem.Data[i];
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
                    }

                    itemDatas.Add(itemData);
                }
                gameData.raceAndEssenceItems = itemDatas;
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadGame: Failed to load inventory data " + ex.Message);
            }
        });
    }
}