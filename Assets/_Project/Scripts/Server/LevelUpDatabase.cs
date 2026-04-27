

using System.Collections.Generic;
using UnityEngine;

public class LevelUpDatabase : Singleton<LevelUpDatabase>
{
    public List<StatsRealmPreset> realmStats;
    public List<SkillPreset> skillPresets = new();
    public List<TechniquePreset> techniquePresets = new();
    private Dictionary<RealmType, RealmData> realmStatsData = new();
    private Dictionary<SkillType, List<SkillData>> skillDatas = new();
    private Dictionary<TechniqueType, List<TechniqueData>> techniqueDatas = new();
    private Dictionary<string, ItemData> itemDataDict = new();
    protected override void Awake()
    {
        base.Awake();
        foreach (var preset in realmStats)
        {
            realmStatsData[preset.realmType] = preset.GetStats();
            if (itemDataDict.ContainsKey(preset.GetStats().instanceId) == false)
                itemDataDict[preset.GetStats().instanceId] = preset.GetStats();
        }
        foreach (var preset in skillPresets)
        {
            if (skillDatas.ContainsKey(preset.skillType) == false)
                skillDatas[preset.skillType] = new List<SkillData>();
            skillDatas[preset.skillType].Add(preset.GetItemData() as SkillData);
            if (itemDataDict.ContainsKey(preset.GetItemData().instanceId) == false)
                itemDataDict[preset.GetItemData().instanceId] = preset.GetItemData();
        }
        foreach (var preset in techniquePresets)
        {
            if (techniqueDatas.ContainsKey(preset.techniqueType) == false)
                techniqueDatas[preset.techniqueType] = new List<TechniqueData>();
            techniqueDatas[preset.techniqueType].Add(preset.GetItemData() as TechniqueData);
            if (itemDataDict.ContainsKey(preset.GetItemData().instanceId) == false)
                itemDataDict[preset.GetItemData().instanceId] = preset.GetItemData();
        }
    }

    public ItemData GetItemDict(string itemId)
    {
        if (itemDataDict.ContainsKey(itemId))
            return itemDataDict[itemId];
        return null;

    }
    #region Get Next Level

    public RealmData GetNextRealm(RealmType realmType)
    {
        RealmType nextRealmType = realmType + 1;
        if (realmType == RealmType.PhiThang)
        {
            nextRealmType = RealmType.PhiThang;
        }

        if (realmStatsData.TryGetValue(nextRealmType, out var stats))
            return stats;

        return null;
    }
    public SkillData GetNextSkillEnhance(string skillid, int currentEnhanceLevel)
    {
        var skillData = itemDataDict[skillid] as SkillData;
        if (skillData == null)
            return null;
        var skillType = skillData.skillType;
        if (skillDatas.TryGetValue(skillType, out var skills))
        {
            return skills.Find(s => s.enhanceLevel == currentEnhanceLevel + 1);
        }
        return null;
    }
    public TechniqueData GetNextTechniqueEnhance(string techniqueId, int currentEnhanceLevel)
    {
        var techniqueData = itemDataDict[techniqueId] as TechniqueData;
        if (techniqueData == null)
            return null;
        var techniqueType = techniqueData.techniqueType;
        if (techniqueDatas.TryGetValue(techniqueType, out var techniques))
        {
            return techniques.Find(t => t.enhanceLevel == currentEnhanceLevel + 1);
        }
        return null;
    }
    #endregion
}