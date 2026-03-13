

using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HeroData : ItemData
{
    public bool isCharactor;
    public StatsRealmData statsRealmData;
    public StatsCultivationPathData statsCultivationPathData;
    public StatsRaceData statsRaceData;
    public RaceType raceType;
    public EssenceType essenceType;
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
    public Vector2Int championIndex;
    public List<EquitmentData> equitmentDatas = new();
    public List<SkillData> skillDatas = new();
    public List<TechniqueData> techniqueDatas = new();
}