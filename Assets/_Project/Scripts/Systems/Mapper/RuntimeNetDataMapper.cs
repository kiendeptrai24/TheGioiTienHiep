using System.Collections.Generic;
using Unity.Collections;

public static class RuntimeNetDataMapper
{
    public static ChampionDataNetDto ToNetDto(HeroData hero)
    {
        if (hero == null)
            return default;

        return new ChampionDataNetDto
        {
            instanceId = ToFixed(hero.instanceId),

            isCharacter = hero.isCharacter,
            manaPersent = hero.manaPersent,
            healthPersent = hero.healthPersent,

            raceId = ToFixed(hero.raceId),
            essenceId = ToFixed(hero.essenceId),
            realmId = ToFixed(hero.realmId),

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
            spiritRangePoint = hero.spiritRangePoint,

            championIndex = hero.championIndex,

            equipmentIds = ToFixedList(hero.equipmentIds),
            skillIds = ToFixedList(hero.skillIds),
            techniqueIds = ToFixedList(hero.techniqueIds)
        };
    }

    public static HeroData ToHeroData(ChampionDataNetDto dto, GameDataCenterManager dataManager)
    {
        if (dataManager == null)
            return null;

        string instanceId = dto.instanceId.ToString();

        HeroData hero = dataManager.GetItemById(instanceId) as HeroData;

        if (hero == null)
            return null;

        hero.championIndex = dto.championIndex;

        hero.healthPersent = dto.healthPersent;
        hero.manaPersent = dto.manaPersent;
        hero.isCharacter = dto.isCharacter;

        hero.raceId = dto.raceId.ToString();
        hero.raceData = dataManager.GetItemById(hero.raceId) as RaceData;

        hero.essenceId = dto.essenceId.ToString();
        hero.essenceData = dataManager.GetItemById(hero.essenceId) as EssenceData;

        hero.realmId = dto.realmId.ToString();
        hero.realmData = dataManager.GetItemById(hero.realmId) as RealmData;

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
        hero.spiritRangePoint = dto.spiritRangePoint;

        hero.equipmentIds = ToStringList(dto.equipmentIds);
        hero.skillIds = ToStringList(dto.skillIds);
        hero.techniqueIds = ToStringList(dto.techniqueIds);

        RebuildEquipmentDatas(hero, dataManager);
        RebuildSkillDatas(hero, dataManager);
        RebuildTechniqueDatas(hero, dataManager);

        return hero;
    }

    public static ItemData GetItemData(string instanceId, GameDataCenterManager dataManager)
    {
        if (dataManager == null)
            return null;

        return dataManager.GetItemById(instanceId);
    }

    private static FixedString64Bytes ToFixed(string value)
    {
        return string.IsNullOrEmpty(value)
            ? default
            : new FixedString64Bytes(value);
    }

    private static FixedList512Bytes<FixedString64Bytes> ToFixedList(List<string> ids)
    {
        var fixedList = new FixedList512Bytes<FixedString64Bytes>();

        if (ids == null)
            return fixedList;

        for (int i = 0; i < ids.Count; i++)
        {
            if (fixedList.Length >= fixedList.Capacity)
                break;

            if (string.IsNullOrEmpty(ids[i]))
                continue;

            fixedList.Add(new FixedString64Bytes(ids[i]));
        }

        return fixedList;
    }

    private static List<string> ToStringList(FixedList512Bytes<FixedString64Bytes> fixedList)
    {
        var list = new List<string>(fixedList.Length);

        for (int i = 0; i < fixedList.Length; i++)
        {
            string value = fixedList[i].ToString();

            if (!string.IsNullOrEmpty(value))
                list.Add(value);
        }

        return list;
    }

    private static void RebuildEquipmentDatas(HeroData hero, GameDataCenterManager dataManager)
    {
        hero.equipmentDatas = new List<EquipmentData>();

        if (hero.equipmentIds == null)
            return;

        foreach (string equipmentId in hero.equipmentIds)
        {
            var item = dataManager.GetItemById(equipmentId);

            if (item == null)
                continue;

            var equipmentData = item.Clone() as EquipmentData;

            if (equipmentData != null)
                hero.equipmentDatas.Add(equipmentData);
        }
    }

    private static void RebuildSkillDatas(HeroData hero, GameDataCenterManager dataManager)
    {
        hero.skillDatas = new List<SkillData>();

        if (hero.skillIds == null)
            return;

        foreach (string skillId in hero.skillIds)
        {
            var item = dataManager.GetItemById(skillId);

            if (item == null)
                continue;

            var skillData = item.Clone() as SkillData;

            if (skillData != null)
                hero.skillDatas.Add(skillData);
        }
    }

    private static void RebuildTechniqueDatas(HeroData hero, GameDataCenterManager dataManager)
    {
        hero.techniqueDatas = new List<TechniqueData>();

        if (hero.techniqueIds == null)
            return;

        foreach (string techniqueId in hero.techniqueIds)
        {
            var item = dataManager.GetItemById(techniqueId);

            if (item == null)
                continue;

            var techniqueData = item.Clone() as TechniqueData;

            if (techniqueData != null)
                hero.techniqueDatas.Add(techniqueData);
        }
    }
}