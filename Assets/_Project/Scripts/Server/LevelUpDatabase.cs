

using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpDatabase : Singleton<LevelUpDatabase>
{
    private Dictionary<RealmType, RealmData> realmStatsData = new();
    private Dictionary<SkillType, List<SkillData>> skillDatas = new();
    private Dictionary<TechniqueType, List<TechniqueData>> techniqueDatas = new();
    private Dictionary<string, ItemData> itemDataDict = new();
    protected override void Awake()
    {
        base.Awake();
        GameDataCenterManager.Instance.OnLoadGameDataCenterSuccessed += OnGameDataReady;
    }

    private void OnGameDataReady(GameDataCenter center)
    {
        var realmDatas = center.realmDatas;
        var skillDatas = center.skillDatas;
        var techniqueDatas = center.techniqueDatas;
        foreach (var realm in realmDatas)
        {
            if (itemDataDict.ContainsKey(realm.instanceId) == false)
            {
                realmStatsData[realm.realmType] = realm;
                itemDataDict[realm.instanceId] = realm;
            }
        }
        foreach (var skill in skillDatas)
        {
            if (itemDataDict.ContainsKey(skill.instanceId) == false)
            {
                if (this.skillDatas.ContainsKey(skill.skillType) == false)
                    this.skillDatas[skill.skillType] = new List<SkillData>();
                this.skillDatas[skill.skillType].Add(skill);
                itemDataDict[skill.instanceId] = skill;
            }
        }
        foreach (var technique in techniqueDatas)
        {
            if (itemDataDict.ContainsKey(technique.instanceId) == false)
            {
                if (this.techniqueDatas.ContainsKey(technique.techniqueType) == false)
                    this.techniqueDatas[technique.techniqueType] = new List<TechniqueData>();
                this.techniqueDatas[technique.techniqueType].Add(technique);
                itemDataDict[technique.instanceId] = technique;
            }
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