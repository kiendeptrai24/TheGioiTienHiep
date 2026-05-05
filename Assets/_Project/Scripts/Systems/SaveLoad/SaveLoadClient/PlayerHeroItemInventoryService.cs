
using System;
using System.Linq;
using UnityEngine;

public class PlayerHeroItemInventoryService :  ILoadRemote<GameData>, ISaveRemote<GameData>
{
    private PlayFabDataClientService service;
    public PlayerHeroItemInventoryService(PlayFabDataClientService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadPlayerHeroData(gameData.characterId, (gameDataDTO) =>
        {
            try
            {
                if (gameDataDTO == null)
                {
                    callback?.Invoke();
                    return;
                }
                var itemsData = new HeroDataDTO();
                itemsData = gameDataDTO;

                var SODataBase = ScriptableObjectLoader.Instance;

                for (int i = 0; i < itemsData.inventoryItems.Count; i++)
                {
                    var item = itemsData.inventoryItems[i];
                    if (item == null)
                    {
                        Debug.Log("item is null");
                        return;
                    }
                    var itemData = SODataBase.GetItem(item.instanceId);
                    itemData.itemName = gameDataDTO.inventoryItems[i].itemName;
                    itemData.realmType = gameDataDTO.inventoryItems[i].realmType;
                    var heroData = itemData as HeroData;
                    if (heroData != null)
                    {
                        SetHeroData(itemsData, SODataBase, i, heroData);
                        if (heroData.isCharactor)
                        {
                            var realmData = SODataBase.GetRealmItem(itemData.realmType);
                            heroData.realmData = realmData;
                            heroData.characterId = gameData.characterId;
                            if (gameData.itemDataPoint != null)
                            {
                                heroData.physicalDamagePoint = gameData.itemDataPoint.damagePoint;
                                heroData.magicalDamagePoint = gameData.itemDataPoint.damagePoint;
                                heroData.spiritDamagePoint = gameData.itemDataPoint.damagePoint;
                                heroData.physicalDefensePoint = gameData.itemDataPoint.defensePoint;
                                heroData.magicalDefensePoint = gameData.itemDataPoint.defensePoint;
                                heroData.spiritDefensePoint = gameData.itemDataPoint.defensePoint;
                                heroData.healthPoint = gameData.itemDataPoint.healthPoint;
                                heroData.manaPoint = gameData.itemDataPoint.manaPoint;
                                heroData.spiritPoint = gameData.itemDataPoint.spiritPoint;
                                heroData.moveSpeedPoint = gameData.itemDataPoint.moveSpeed;
                                heroData.spititRangePoint = gameData.itemDataPoint.spititRange;
                                heroData.potentialPoint = gameData.potentialPoint;
                                heroData.skillPoint = gameData.skillPoint;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                    itemsData.inventoryItems[i] = heroData;
                }
                gameData.itemDatas.AddRange(itemsData.inventoryItems.ToList<ItemData>());
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error occurred while loading item data." + ex.Message);
            }
        });
    }

    private static void SetHeroData(HeroDataDTO itemsData, ScriptableObjectLoader SODataBase, int i, HeroData heroData)
    {
        try
        {
            heroData.skillDatas.Clear();
            heroData.techniqueDatas.Clear();
            heroData.equipmentDatas.Clear();
            var champion = itemsData.inventoryItems[i];

            if (champion == null)
            {
                Debug.Log("champion is null");
                return;
            }

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
                var equipmentData = SODataBase.GetItem(equipment.instanceId) as EquipmentData;
                if (equipmentData == null)
                    continue;
                heroData.equipmentDatas.Add(equipmentData);
            }
            var statRace = SODataBase.GetRaceItem(heroData.raceType);
            if (statRace != null)
                heroData.raceData = statRace;
            itemsData.inventoryItems[i] = heroData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SetHeroData: Failed to set hero data " + ex.Message);
        }
    }
    public void SaveGame(GameData gameData)
    {
        service.SavePlayerHeroData(gameData);
    }
}