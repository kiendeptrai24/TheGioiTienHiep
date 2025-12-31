using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroPreset", menuName = "RPG/Items/Hero Preset")]
public class HeroPreset : ItemStatsPreset
{
    public StatsRealmPreset statsRealmPreset;
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
    public List<SkillPreset> skillDatas;
    public List<TechniquePreset> techniqueDatas;
}