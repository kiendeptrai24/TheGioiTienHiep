using System;
using UnityEngine;

public class LoadChampionInventoryClient : ILoadGameData<GameData, PlayerClientDataDto>
{
    public void LoadGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (playerClientDataDto == null || playerClientDataDto.championInInventoryRes == null)
            {
                Debug.LogWarning("LoadChampionInventoryClient: championInInventoryRes is null");
                return;
            }

            var dataManager = GameDataCenterManager.Instance;
            foreach (var championDto in playerClientDataDto.championInInventoryRes)
            {

                var heroDataBase = dataManager.GetItemById(championDto.instanceId) as HeroData;
                if (heroDataBase != null)
                {
                    IsCharacter(dataManager, championDto, heroDataBase);
                    if (championDto.posX.HasValue && championDto.posY.HasValue)
                    {
                        int posX = championDto.posX.Value;
                        int posY = championDto.posY.Value;
                        heroDataBase.championIndex = new Vector2Int(posX, posY);
                    }
                    gameData.itemDatas.Add(heroDataBase);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadChampionInventoryClient failed: {ex.Message}");
        }
    }

    private void IsCharacter(GameDataCenterManager dataManager, ChampionDataDto championDto, HeroData heroData)
    {
        if (championDto.isCharacter.HasValue && championDto.isCharacter.Value)
        {
            heroData.itemName = championDto.name;
            var realmDataBase = dataManager.GetItemById(championDto.realmId) as RealmData;
            var raceDataBase = dataManager.GetItemById(championDto.raceId) as RaceData;
            var essenceDataBase = dataManager.GetItemById(championDto.essenceId) as EssenceData;
            if (realmDataBase != null)
            {
                heroData.realmId = realmDataBase.instanceId;
                heroData.realmData = realmDataBase;
                heroData.realmType = realmDataBase.realmType;
            }
            if (raceDataBase != null)
            {
                heroData.raceId = raceDataBase.instanceId;
                heroData.raceData = raceDataBase;
                heroData.raceType = raceDataBase.raceType;
            }
            if (essenceDataBase != null)
            {
                heroData.essenceId = essenceDataBase.instanceId;
                heroData.essenceData = essenceDataBase;
                heroData.essenceType = essenceDataBase.essenceType;
            }
        }
        LoadEquipment(heroData, championDto);
    }

    private void LoadEquipment(HeroData heroData, ChampionDataDto championDto)
    {
        if (heroData == null || heroData.equipmentIds == null)
        {
            Debug.LogWarning("LoadEquipment: heroData or equipmentIds is null");
            return;
        }
        var dataManager = GameDataCenterManager.Instance;
        foreach (var equipmentId in championDto.equipmentIds)
        {
            var equipmentData = dataManager.GetItemById(equipmentId) as EquipmentData;
            if (equipmentData != null)
            {
                heroData.equipmentIds.Add(equipmentId);
                heroData.equipmentDatas.Add(equipmentData);
            }
        }
        foreach (var techniqueId in championDto.techniqueIds)
        {
            var techniqueData = dataManager.GetItemById(techniqueId) as TechniqueData;
            if (techniqueData != null)
            {
                heroData.techniqueIds.Add(techniqueId);
                heroData.techniqueDatas.Add(techniqueData);
            }
        }
        foreach (var skillId in championDto.skillIds)
        {
            var skillData = dataManager.GetItemById(skillId) as SkillData;
            if (skillData != null)
            {
                heroData.skillIds.Add(skillId);
                heroData.skillDatas.Add(skillData);
            }
        }
    }
}