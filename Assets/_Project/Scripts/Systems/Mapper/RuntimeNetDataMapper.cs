using System.Collections.Generic;
using Unity.Collections;

public static class RuntimeNetDataMapper
{
    private const int SPLIT_SIZE = 7; // FixedList512Bytes chứa tối đa 7 FixedString64Bytes

    public static ChampionDataNetDto ToNetDto(HeroData hero)
    {
        if (hero == null)
            return default;

        SplitToFixedLists(hero.equipmentIds, out var equipA, out var equipB);
        SplitToFixedLists(hero.skillIds, out var skillA, out var skillB);
        SplitToFixedLists(hero.techniqueIds, out var techA, out var techB);

        return new ChampionDataNetDto
        {
            instanceId = ToFixed(hero.instanceId),
            name = ToFixed(hero.itemName),

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

            equipmentIds = equipA,
            equipmentIds1 = equipB,
            skillIds = skillA,
            skillIds1 = skillB,
            techniqueIds = techA,
            techniqueIds1 = techB,
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
        hero.itemName = dto.name.ToString();
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

        hero.equipmentIds = MergeToStringList(dto.equipmentIds, dto.equipmentIds1);
        hero.skillIds = MergeToStringList(dto.skillIds, dto.skillIds1);
        hero.techniqueIds = MergeToStringList(dto.techniqueIds, dto.techniqueIds1);

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

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static FixedString64Bytes ToFixed(string value) =>
        string.IsNullOrEmpty(value) ? default : new FixedString64Bytes(value);

    /// <summary>
    /// Split List<string> thành 2 FixedList512Bytes (mỗi cái tối đa SPLIT_SIZE items).
    /// Đủ chứa tổng 14 items; chỉnh SPLIT_SIZE nếu cần nhiều hơn.
    /// </summary>
    private static void SplitToFixedLists(
        List<string> ids,
        out FixedList512Bytes<FixedString64Bytes> partA,
        out FixedList512Bytes<FixedString64Bytes> partB)
    {
        partA = new FixedList512Bytes<FixedString64Bytes>();
        partB = new FixedList512Bytes<FixedString64Bytes>();

        if (ids == null) return;

        for (int i = 0; i < ids.Count; i++)
        {
            if (string.IsNullOrEmpty(ids[i])) continue;

            var fixedStr = new FixedString64Bytes(ids[i]);

            if (i < SPLIT_SIZE)
                partA.Add(fixedStr);
            else
                partB.Add(fixedStr);
        }
    }

    /// <summary>
    /// Merge 2 FixedList512Bytes trở lại thành List<string>.
    /// Fix bug code gốc (đọc nhầm partA thay vì partB ở vòng 2).
    /// </summary>
    private static List<string> MergeToStringList(
        FixedList512Bytes<FixedString64Bytes> partA,
        FixedList512Bytes<FixedString64Bytes> partB)
    {
        var list = new List<string>(partA.Length + partB.Length);

        for (int i = 0; i < partA.Length; i++)
        {
            string v = partA[i].ToString();
            if (!string.IsNullOrEmpty(v)) list.Add(v);
        }

        for (int i = 0; i < partB.Length; i++)
        {
            string v = partB[i].ToString();  // fix: partB[i], không phải partA[i]
            if (!string.IsNullOrEmpty(v)) list.Add(v);
        }

        return list;
    }

    // ─── Rebuild ─────────────────────────────────────────────────────────────

    private static void RebuildEquipmentDatas(HeroData hero, GameDataCenterManager dataManager)
    {
        hero.equipmentDatas = new List<EquipmentData>();
        if (hero.equipmentIds == null) return;

        foreach (string id in hero.equipmentIds)
        {
            var equipmentData = dataManager.GetItemById(id) as EquipmentData;
            if (equipmentData != null) hero.equipmentDatas.Add(equipmentData);
        }
    }

    private static void RebuildSkillDatas(HeroData hero, GameDataCenterManager dataManager)
    {
        hero.skillDatas = new List<SkillData>();
        if (hero.skillIds == null) return;

        foreach (string id in hero.skillIds)
        {
            var skillData = dataManager.GetItemById(id) as SkillData;
            if (skillData != null) hero.skillDatas.Add(skillData);
        }
    }

    private static void RebuildTechniqueDatas(HeroData hero, GameDataCenterManager dataManager)
    {
        hero.techniqueDatas = new List<TechniqueData>();
        if (hero.techniqueIds == null) return;

        foreach (string id in hero.techniqueIds)
        {
            var techniqueData = dataManager.GetItemById(id) as TechniqueData;
            if (techniqueData != null) hero.techniqueDatas.Add(techniqueData);
        }
    }
}