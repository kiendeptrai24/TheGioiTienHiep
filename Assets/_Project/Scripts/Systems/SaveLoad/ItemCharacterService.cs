
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemCharacterService : ISaveLoadRemote
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

                var SODataBase = ScriptableObjectLoader.Instance;

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

                    var itemData = SODataBase.GetItem(itemLoad.instanceId);
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
                        if (itemsData.characterNames != null && i < itemsData.characterNames.Count)
                        {
                            heroData.itemName = itemsData.characterNames[i];
                        }

                        if (itemsData.characterIds != null && i < itemsData.characterIds.Count)
                        {
                            heroData.characterId = itemsData.characterIds[i];
                        }

                        var realmData = SODataBase.GetRealmItem(itemData.realmType);
                        heroData.realmData = realmData;

                        SetHeroData(itemsData, SODataBase, i, heroData);
                        continue;
                    }

                    itemsData.inventoryItems[i] = heroData;
                }
                var listItem = new List<ItemData>();
                foreach (var item in itemsData.inventoryItems)
                {
                    listItem.Add(item);
                }
                gameData.itemDatasCharacter = listItem;
                callback?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"LoadGame: Failed to load character data\n{ex}");
            }
        });
    }

    private static void SetHeroData(ItemCharacterDataDTO itemsData, ScriptableObjectLoader SODataBase, int i, HeroData heroData)
    {
        try
        {
            var champion = itemsData.inventoryItems[i];
            if (champion == null)
                return;
            try
            {
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
                    var skillData = SODataBase.GetItem(skill.instanceId) as SkillData;
                    if (skillData == null)
                        continue;
                    heroData.skillDatas[h] = skillData;
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("SetHeroData: Failed to set skill data ");
            }

            var techniqueDatas = champion.techniqueDatas;
            for (int s = 0; s < techniqueDatas.Count; s++)
            {
                var technique = techniqueDatas[s];
                var techniqueData = SODataBase.GetItem(technique.instanceId) as TechniqueData;
                if (techniqueData == null)
                    continue;
                heroData.techniqueDatas[s] = techniqueData;
            }
            var equipmentDatas = champion.equipmentDatas;
            for (int k = 0; k < equipmentDatas.Count; k++)
            {
                var equipment = equipmentDatas[k];
                var equipmentData = SODataBase.GetItem(equipment.instanceId) as EquitmentData;
                if (equipmentData == null)
                    continue;
                heroData.equipmentDatas[k] = equipmentData;
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
        service.SetItemCharacter(gameData);
    }
}