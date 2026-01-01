using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroPreset", menuName = "RPG/Items/Hero Preset")]
public class HeroPreset : ItemStatsPreset
{
    public StatsRealmPreset statsRealmPreset;
    public StatsRacePreset statsRacePreset;
    public StatsCultivationPathPreset statsCultivationPathPreset;
    public RaceType raceType;
    public EssenceType essenceType;
    public ElementType elementType;
    public int level;
    public float attackRange;
    public float health; // persent
    public float mana; // persent
    public float spirit; // persent
    public float physicalDamagePoint; // value
    public float magicalDamagePoint; // value
    public float spiritDamagePoint; // value
    public float physicalDefensePoint; // value
    public float magicalDefensePoint; // value
    public float spiritDefensePoint; // value
    public List<ItemEquipmentPreset> equitmentDatas;
    public List<SkillPreset> skillDatas;
    public List<TechniquePreset> techniqueDatas;
    public override ItemData GetItemData()
    {
        HeroData heroPreset = new HeroData
        {
            itemId = itemId,
            itemName = itemName,
            itemType = itemType,
            itemIcon = itemIcon,
            itemDescription = itemDescription,
            currentstack = currentstack,
            
            level = level,
            attackRange = attackRange,
            health = health,
            mana = mana,
            spirit = spirit,
            physicalDamagePoint = physicalDamagePoint,
            magicalDamagePoint = magicalDamagePoint,
            spiritDamagePoint = spiritDamagePoint,
            physicalDefensePoint = physicalDefensePoint,
            magicalDefensePoint = magicalDefensePoint,
            spiritDefensePoint = spiritDefensePoint,
            qualityType = qualityType,
        };
        heroPreset.statsRealmData = statsRealmPreset.GetStats();
        heroPreset.statsRaceData = statsRacePreset.GetStats();
        heroPreset.statsCultivationPathData = statsCultivationPathPreset.GetStats();

        heroPreset.essenceType = essenceType;
        heroPreset.elementType = elementType;
        heroPreset.equitmentDatas = GetEquitmentDatas();
        heroPreset.skillDatas = GetSkillDatas();
        heroPreset.techniqueDatas = GetTechniqueDatas();
        return heroPreset;
    }
    public List<SkillData> GetSkillDatas()
    {
        List<SkillData> skills = new List<SkillData>();
        foreach (var item in skillDatas)
        {
            var skill = item.GetItemData();
            skills.Add((SkillData)skill);
        }
        return skills;
    }
    public List<TechniqueData> GetTechniqueDatas()
    {
        List<TechniqueData> techniques = new List<TechniqueData>();
        foreach (var item in techniqueDatas)
        {
            var technique = item.GetItemData();
            techniques.Add((TechniqueData)technique);
        }
        return techniques;
    }
    public List<EquitmentData> GetEquitmentDatas()
    {
        List<EquitmentData> equitments = new List<EquitmentData>();
        foreach (var item in equitmentDatas)
        {
            var equitment = item.GetItemData();
            equitments.Add((EquitmentData)equitment);
        }
        return equitments;
    }
}