

using System.Collections.Generic;

public class HeroData : ItemData
{
    public StatsRealmData statsRealmData;
    public RaceType raceType;
    public EssenceType essenceType;
    public ElementType elementType;
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
    public List<TechniqueType> techniques;
    public List<SkillData> skillDatas;
    public List<SkillType> skills;
    public List<TechniqueData> techniqueDatas;

}