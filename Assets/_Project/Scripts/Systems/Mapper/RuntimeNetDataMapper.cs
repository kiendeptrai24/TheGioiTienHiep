using System.Collections.Generic;

public static class RuntimeNetDataMapper
{
    // ItemData -> BaseDataNetDto
    public static BaseDataNetDto ToNetDto(ItemData item)
    {
        if (item == null) return null;

        return new BaseDataNetDto
        {
            instanceId = item.instanceId
        };
    }

    public static ItemData ToItemData(BaseDataNetDto dto, GameDataCenterManager dataManager)
    {
        if (dto == null) return null;
        ItemData item = dataManager.GetItemById(dto.instanceId);
        return item;
    }

    // HeroData -> ChampionDataNetDto
    public static ChampionDataNetDto ToNetDto(HeroData hero)
    {
        if (hero == null) return null;

        var itemDto = new ChampionDataNetDto
        {
            instanceId = hero.instanceId,

            isCharacter = hero.isCharacter,
            raceId = hero.raceId,
            essenceId = hero.essenceId,
            realmId = hero.realmId,

            physicalDamagePoint = hero.physicalDamagePoint,
            magicalDamagePoint = hero.magicalDamagePoint,
            spiritDamagePoint = hero.spiritDamagePoint,

            physicalDefensePoint = hero.physicalDefensePoint,
            magicalDefensePoint = hero.magicalDefensePoint,
            spiritDefensePoint = hero.spiritDefensePoint,

            healthPoint = hero.healthPoint,
            manaPoint = hero.manaPoint,
            spiritPoint = hero.spiritPoint,

            moveSpeedPoint = hero.moveSpeedPoint,
            spititRangePoint = hero.spiritRangePoint,

            championIndex = hero.championIndex,

            equipmentIds = hero.equipmentIds != null
                ? new List<string>(hero.equipmentIds)
                : new List<string>(),

            skillIds = hero.skillIds != null
                ? new List<string>(hero.skillIds)
                : new List<string>(),

            techniqueIds = hero.techniqueIds != null
                ? new List<string>(hero.techniqueIds)
                : new List<string>()
        };
        return itemDto;
    }

    // ChampionDataNetDto -> HeroData
    public static HeroData ApplyToHero(ChampionDataNetDto dto, GameDataCenterManager dataManager)
    {
        if (dto == null) return null;

        HeroData hero = dataManager.GetItemById(dto.instanceId) as HeroData;
        hero.championIndex = dto.championIndex;
        hero.isCharacter = dto.isCharacter;
        hero.raceId = dto.raceId;
        hero.raceData = dataManager.GetItemById(dto.raceId) as RaceData;
        hero.essenceId = dto.essenceId;
        hero.essenceData = dataManager.GetItemById(dto.essenceId) as EssenceData;
        hero.realmId = dto.realmId;
        hero.realmData = dataManager.GetItemById(dto.realmId) as RealmData;

        hero.physicalDamagePoint = dto.physicalDamagePoint;
        hero.magicalDamagePoint = dto.magicalDamagePoint;
        hero.spiritDamagePoint = dto.spiritDamagePoint;

        hero.physicalDefensePoint = dto.physicalDefensePoint;
        hero.magicalDefensePoint = dto.magicalDefensePoint;
        hero.spiritDefensePoint = dto.spiritDefensePoint;

        hero.healthPoint = dto.healthPoint;
        hero.manaPoint = dto.manaPoint;
        hero.spiritPoint = dto.spiritPoint;

        hero.moveSpeedPoint = dto.moveSpeedPoint;
        hero.spiritRangePoint = dto.spititRangePoint;

        hero.skillIds = dto.skillIds != null
            ? new List<string>(dto.skillIds)
            : new List<string>();
        hero.skillDatas = new List<SkillData>();

        foreach (var skillId in hero.skillIds)
        {
            var skillData = dataManager.GetItemById(skillId).Clone() as SkillData;
            if (skillData != null)
            {
                hero.skillDatas.Add(skillData);
            }
        }

        hero.techniqueIds = dto.techniqueIds != null
            ? new List<string>(dto.techniqueIds)
            : new List<string>();
        hero.techniqueDatas = new List<TechniqueData>();
        foreach (var techniqueId in hero.techniqueIds)
        {
            var techniqueData = dataManager.GetItemById(techniqueId).Clone() as TechniqueData;
            if (techniqueData != null)
            {
                hero.techniqueDatas.Add(techniqueData);
            }
        }

        hero.equipmentIds = dto.equipmentIds != null
            ? new List<string>(dto.equipmentIds)
            : new List<string>();
        hero.equipmentDatas = new List<EquipmentData>();
        foreach (var equipmentId in hero.equipmentIds)
        {
            var equipmentData = dataManager.GetItemById(equipmentId).Clone() as EquipmentData;
            if (equipmentData != null)
            {
                hero.equipmentDatas.Add(equipmentData);
            }
        }
        return hero;
    }
    public static ItemData GetItemData(string instanceId, GameDataCenterManager dataManager) => dataManager.GetItemById(instanceId);
    public static HeroData ToHeroData(ChampionDataNetDto dto, GameDataCenterManager dataManager)
    {
        if (dto == null) return null;

        HeroData hero = ApplyToHero(dto, dataManager);
        return hero;
    }
}