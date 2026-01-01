

using System.Collections.Generic;

public class HeroData : ItemData
{
    public StatsRealmData statsRealmData;
    public StatsCultivationPathData statsCultivationPathData;
    public StatsRaceData statsRaceData;
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
    public List<EquitmentData> equitmentDatas;
    public List<SkillData> skillDatas;
    public List<TechniqueData> techniqueDatas;
}