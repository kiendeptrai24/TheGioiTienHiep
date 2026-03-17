using System.Collections.Generic;
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
    public float physicalDamagePoint; // value
    public float magicalDamagePoint; // value
    public float spiritDamagePoint; // value
    public float physicalDefensePoint; // value
    public float magicalDefensePoint; // value
    public float spiritDefensePoint; // value
    public List<ItemEquipmentPreset> equitmentDatas;
    public List<SkillPreset> skillDatas;
    public List<TechniquePreset> techniqueDatas;
    public Vector2Int championIndex = new Vector2Int(0, 0);
    public GameObject heroPrefab;

    public override ItemData GetItemData()
    {
        ItemData data = base.GetItemData();
        HeroData heroPreset = new HeroData
        {
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
            physicalDamagePoint = physicalDamagePoint,
            magicalDamagePoint = magicalDamagePoint,
            spiritDamagePoint = spiritDamagePoint,
            physicalDefensePoint = physicalDefensePoint,
            magicalDefensePoint = magicalDefensePoint,
            spiritDefensePoint = spiritDefensePoint,
            championIndex = championIndex,
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