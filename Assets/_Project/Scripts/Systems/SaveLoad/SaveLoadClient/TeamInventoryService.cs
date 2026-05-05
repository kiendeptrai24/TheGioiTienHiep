
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class TeamInventoryService : ISaveLoadRemote
{
    private PlayFabDataClientService service;
    public TeamInventoryService(PlayFabDataClientService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadTeamData(gameData.characterId, (gameDataDTO) =>
        {
            try
            {
                if (gameDataDTO == null)
                {
                    callback?.Invoke();
                    return;
                }
                var itemTeam = new HeroInTeamDataDTO();
                itemTeam = gameDataDTO;

                var dataManager = GameDataCenterManager.Instance;

                for (int i = 0; i < itemTeam.inventoryItems.Count; i++)
                {
                    var item = itemTeam.inventoryItems[i];
                    var itemData = dataManager.GetItemById(item.instanceId);
                    itemData.itemName = gameDataDTO.inventoryItems[i].itemName;
                    itemData.realmType = gameDataDTO.inventoryItems[i].realmType;
                    if (itemData == null)
                        continue;
                    var heroData = itemData as HeroData;
                    if (heroData != null)
                    {
                        heroData.championIndex = itemTeam.championsIndex[i];
                        SetHeroData(itemTeam, dataManager, i, heroData);
                        if (heroData.isCharactor)
                        {
                            var realmData = dataManager.GetItemById(itemData.realmId) as RealmData;
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
                        continue;
                    }

                    itemTeam.inventoryItems[i] = heroData;
                }
                var listItem = new List<ItemData>();
                foreach (var item in itemTeam.inventoryItems)
                {
                    listItem.Add(item);
                }
                gameData.itemInTeamDatas = listItem;
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadGame: Failed to load team inventory data " + ex.Message);
            }
        });
    }

    private void SetHeroData(HeroInTeamDataDTO itemsteam, GameDataCenterManager dataManager, int i, HeroData heroData)
    {
        try
        {
            heroData.skillDatas.Clear();
            heroData.techniqueDatas.Clear();
            heroData.equipmentDatas.Clear();

            var champion = itemsteam.inventoryItems[i];

            if (champion == null)
            {
                Debug.Log("champion is null");
                return;
            }

            var skillDatas = champion.skillDatas;
            for (int h = 0; h < skillDatas.Count; h++)
            {
                var skill = skillDatas[h];

                var skillData = dataManager.GetItemById(skill.instanceId) as SkillData;
                if (skillData == null)
                    continue;
                heroData.skillDatas.Add(skillData);
            }

            var techniqueDatas = champion.techniqueDatas;
            for (int s = 0; s < techniqueDatas.Count; s++)
            {
                var technique = techniqueDatas[s];
                var techniqueData = dataManager.GetItemById(technique.instanceId) as TechniqueData;
                if (techniqueData == null)
                    continue;
                heroData.techniqueDatas.Add(techniqueData);
            }
            var equipmentDatas = champion.equipmentDatas;
            for (int k = 0; k < equipmentDatas.Count; k++)
            {
                var equipment = equipmentDatas[k];
                var equipmentData = dataManager.GetItemById(equipment.instanceId) as EquipmentData;
                if (equipmentData == null)
                    continue;
                heroData.equipmentDatas.Add(equipmentData);
            }
            var statRace = dataManager.GetItemById(heroData.raceId) as RaceData;
            if (statRace != null)
                heroData.raceData = statRace;

            itemsteam.inventoryItems[i] = heroData;

        }
        catch (System.Exception ex)
        {
            Debug.LogError("SetHeroData: Failed to set hero data " + ex.Message);
        }
    }
    public void SaveGame(GameData gameData)
    {
        service.SetTeamData(gameData);
    }
}