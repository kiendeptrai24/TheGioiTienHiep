using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadChampionTeamClient : ILoadGameData<GameData, PlayerClientDataDto>
{
    public void LoadGameData(GameData gameData, PlayerClientDataDto playerClientDataDto)
    {
        try
        {
            if (playerClientDataDto == null || playerClientDataDto.championInTeamRes == null)
            {
                Debug.LogWarning("LoadChampionTeamClient: championInTeamRes is null");
                return;
            }

            var dataManager = GameDataCenterManager.Instance;
            foreach (var championDto in playerClientDataDto.championInTeamRes)
            {
                var heroData = dataManager.GetItemById(championDto.instanceId) as HeroData;
                if (heroData != null)
                {
                    IsCharacter(dataManager, championDto, heroData);
                    // Set other hero properties as needed
                    if (championDto.posX.HasValue && championDto.posY.HasValue)
                    {
                        int posX = championDto.posX.Value;
                        int posY = championDto.posY.Value;
                        heroData.championIndex = new Vector2Int(posX, posY);
                    }
                    gameData.itemChampionInTeamDatas.Add(heroData);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadChampionTeamClient failed: {ex.Message}");
        }
    }

    private void IsCharacter(GameDataCenterManager dataManager, ChampionDataDto championDto, HeroData heroData)
    {
        if (championDto.isCharacter.HasValue && championDto.isCharacter.Value)
        {
            heroData.itemName = championDto.name;
            heroData.characterId = championDto.characterId;
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
        if (championDto.isCharacter.HasValue && championDto.isCharacter.Value)
        {
            heroData.skillIds.Clear();
            heroData.skillDatas.Clear();
            heroData.techniqueIds.Clear();
            heroData.techniqueDatas.Clear();
            foreach (var skillId in championDto.skillIds)
            {
                var skillData = dataManager.GetItemById(skillId) as SkillData;
                if (skillData != null)
                {
                    heroData.skillIds.Add(skillId);
                    heroData.skillDatas.Add(skillData);
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
        }
        heroData.equipmentIds.Clear();
        heroData.equipmentDatas.Clear();
        foreach (var equipmentId in championDto.equipmentIds)
        {
            var equipmentData = dataManager.GetItemById(equipmentId) as EquipmentData;
            if (equipmentData != null)
            {
                heroData.equipmentIds.Add(equipmentId);
                heroData.equipmentDatas.Add(equipmentData);
            }
        }
    }
}