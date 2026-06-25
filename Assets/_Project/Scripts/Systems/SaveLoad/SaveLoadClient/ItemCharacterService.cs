
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemCharacterService : ILoadRemote<GameData>, ISaveRemote<GameData>
{
    private PlayFabDataClientService service;
    public ItemCharacterService(PlayFabDataClientService service)
    {
        this.service = service;
    }

    public void LoadGame(GameData gameData, Action callback)
    {
        service.LoadCharacter((gameDataDTO) =>
        {
            try
            {
                if (gameDataDTO == null)
                {
                    callback?.Invoke();
                    return;
                }

                var itemsData = gameDataDTO;

                var dataManager = GameDataCenterManager.Instance;

                if (itemsData.inventoryItems == null)
                {
                    Debug.LogError("LoadGame: inventoryItems is null");
                    callback?.Invoke();
                    return;
                }

                for (int i = 0; i < itemsData.inventoryItems.Count; i++)
                {
                    var itemLoad = itemsData.inventoryItems[i];
                    if (itemLoad == null)
                        continue;

                    var itemData = dataManager.GetItemById(itemLoad.instanceId);
                    if (itemData == null)
                    {
                        Debug.LogError($"LoadGame: itemData null at index {i}, instanceId = {itemLoad.instanceId}");
                        continue;
                    }

                    if (gameDataDTO.inventoryItems != null && i < gameDataDTO.inventoryItems.Count)
                    {
                        itemData.itemName = gameDataDTO.inventoryItems[i].itemName;
                    }

                    var heroData = itemData as HeroData;
                    if (heroData != null)
                    {
                        heroData.essenceId = gameDataDTO.inventoryItems[i].essenceId;
                        if (itemsData.characterNames != null && i < itemsData.characterNames.Count)
                        {
                            heroData.itemName = itemsData.characterNames[i];
                        }

                        if (itemsData.characterIds != null && i < itemsData.characterIds.Count)
                        {
                            heroData.characterId = itemsData.characterIds[i];
                        }

                        var realmData = dataManager.GetItemById(itemData.realmId) as RealmData;
                        if (realmData == null)
                        {
                            Debug.LogError($"LoadGame: realmData null for hero at index {i}, realmId = {itemData.realmId}");
                            continue;
                        }
                        heroData.realmData = realmData;

                        SetHeroData(itemsData, dataManager, i, heroData);
                        continue;
                    }

                    itemsData.inventoryItems[i] = heroData;
                }
                var listItem = new List<ItemData>();
                foreach (var item in itemsData.inventoryItems)
                {
                    listItem.Add(item);
                }
                gameData.itemCharacterDatas = listItem;
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"LoadGame: Failed to load character data\n{ex}");
            }
        });
    }

    private static void SetHeroData(ItemCharacterDataDTO itemsData, GameDataCenterManager dataManager, int i, HeroData heroData)
    {
        try
        {
            var champion = itemsData.inventoryItems[i];
            if (champion == null)
                return;
            try
            {
                var skillDatasTemps = new List<SkillData>();
                if (champion == null)
                {
                    Debug.Log("champion is null");
                    return;
                }
                var skillDatas = champion.skillDatas;
                if (skillDatas == null)
                {
                    Debug.Log("skillDatas is null");
                    return;
                }
                for (int h = 0; h < skillDatas.Count; h++)
                {
                    var skill = skillDatas[h];
                    if (skill == null)
                    {
                        Debug.Log("skill is null");
                        continue;
                    }
                    var skillData = dataManager.GetItemById(skill.instanceId) as SkillData;
                    if (skillData == null)
                        continue;
                    skillDatasTemps.Add(skillData);
                }
                heroData.skillDatas = skillDatasTemps;
            }
            catch (System.Exception)
            {
                Debug.LogError("SetHeroData: Failed to set skill data ");
            }
            try
            {

                var techniqueDatas = champion.techniqueDatas;
                var techniqueDatasTemps = new List<TechniqueData>();
                for (int s = 0; s < techniqueDatas.Count; s++)
                {
                    var technique = techniqueDatas[s];
                    var techniqueData = dataManager.GetItemById(technique.instanceId) as TechniqueData;
                    if (techniqueData == null)
                        continue;
                    techniqueDatasTemps.Add(techniqueData);
                }
                heroData.techniqueDatas = techniqueDatasTemps;
            }
            catch
            {
                Debug.LogError("SetHeroData: Failed to set technique data ");
            }
            try
            {
                var equipmentDatas = champion.equipmentDatas;
                var equipmentDatasTemps = new List<EquipmentData>();
                for (int k = 0; k < equipmentDatas.Count; k++)
                {
                    var equipment = equipmentDatas[k];
                    var equipmentData = dataManager.GetItemById(equipment.instanceId) as EquipmentData;
                    if (equipmentData == null)
                        continue;
                    equipmentDatasTemps.Add(equipmentData);
                }
                heroData.equipmentDatas = equipmentDatasTemps;
            }
            catch
            {
                Debug.LogError("SetHeroData: Failed to set equipment data ");
            }
            var statRace = dataManager.GetItemById(heroData.raceId) as RaceData;
            if (statRace != null)
                heroData.raceData = statRace;
            itemsData.inventoryItems[i] = heroData;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SetHeroData: Failed to set hero data " + ex.Message);
        }
    }
    public void SaveGame(GameData gameData, Action<bool> onCompleted = null)
    {
        service.SetItemCharacter(gameData, onCompleted);
    }
}
