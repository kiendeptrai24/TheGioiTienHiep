using System.Collections.Generic;
using NUnit.Framework;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroPreset", menuName = "RPG/Items/Hero Preset")]
public class HeroPreset : ItemStatsPreset
{
    public bool isCharactor = false;
    public StatsRealmPreset statsRealmPreset;
    public StatsRacePreset statsRacePreset;
    public StatsCultivationPathPreset statsCultivationPathPreset;
    public RaceType raceType;
    public EssenceType essenceType;
    public ElementType elementType;
    public int level;
    public float attackRange;
    public float moveSpeed;
    public float health; // persent
    public float mana; // persent
    public float spirit; // persent
    public List<ItemEquipmentPreset> equitmentDatas;
    public List<SkillPreset> skillDatas;
    public List<TechniquePreset> techniqueDatas;
    public Vector2Int championIndex = new Vector2Int(0, 0);
    public GameObject heroPrefab;
    public override void OnValidate()
    {
#if UNITY_EDITOR
        base.OnValidate();
        if (heroPrefab != null)
        {
            itemFilePath = heroPrefab.name;
        }
#endif
    }
    public override ItemData GetItemData()
    {
        ItemData data = base.GetItemData();
        HeroData heroData = new HeroData
        {
            instanceId = data.instanceId,
            itemId = data.itemId,
            itemName = data.itemName,
            itemType = data.itemType,
            itemIcon = data.itemIcon,
            itemDescription = data.itemDescription,
            currentstack = data.currentstack,
            itemIconPath = data.itemIconPath,
            canStack = data.canStack,
            itemPrice = data.itemPrice,
            realmType = data.realmType,
            qualityType = data.qualityType,

            isCharactor = isCharactor,
            raceType = raceType,
            level = level,
            attackRange = attackRange,
            moveSpeed = moveSpeed,
            health = health,
            mana = mana,
            spirit = spirit,
            championIndex = championIndex,
            heroPrefab = heroPrefab,
            itemFilePath = itemFilePath
        };
        heroData.realmData = statsRealmPreset.GetStats();
        heroData.raceData = statsRacePreset.GetStats();
        heroData.statsCultivationPathData = statsCultivationPathPreset.GetStats();

        heroData.essenceType = essenceType;
        heroData.elementType = elementType;
        heroData.equipmentDatas = GetEquitmentDatas();
        heroData.skillDatas = GetSkillDatas();
        heroData.techniqueDatas = GetTechniqueDatas();
        return heroData;
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