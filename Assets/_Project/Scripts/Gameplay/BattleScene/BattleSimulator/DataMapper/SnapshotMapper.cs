using System.Collections.Generic;
using UnityEngine;

public static class SnapshotMapper
{
    public static UnitInput FromStats(StatsDataCore s, TeamId team, float persent = 1f)
    {
        int hpMax = s.Health;
        int hp = Mathf.RoundToInt(hpMax * persent);

        var heroData = s.heroData as HeroData;
        List<SkillData> skills = heroData != null ? heroData.skillDatas : new List<SkillData>();
        return new UnitInput
        {
            snap = new UnitSnapshot
            {
                uid = heroData.instanceId,
                team = team,
                hpMax = hpMax,
                hp = hp,
                manaMax = s.Mana,
                mana = s.Mana,

                physicalDmg = s.PhysicalDamage,
                magicalDmg = s.MagicalDamage,
                spiritDmg = s.SpiritDamage,
                trueDmg = s.TrueDamage,

                armorPen = s.ArmorPenetration,
                spiritPen = s.SpiritPenetration,

                physicalDef = s.PhysicalDefense,
                magicalDef = s.MagicalDefense,
                spiritDef = s.SpiritDefense,

                critChance = s.GetStatValue(StatType.CritChance) / 100f,
                critPower = 2f + s.GetStatValue(StatType.CritPower) / 100f,
                critReduction = s.CritDamageReduction,

                penReduction = s.PenetrationDamageReduction,
                trueReduction = s.TrueDamageReduction,
                dmgImmunity = s.DamageImmunity,

                lifeSteal = s.LifeSteal,
                reflect = s.ReflectDamage,

                attackSpeed = Mathf.Max(1, s.AttackSpeed),
                attackRange = Mathf.Max(1, heroData.attackRange),
                moveSpeed = Mathf.Max(1, s.MovementSpeed),
                animationDuration = 1f,
                castTime = .5f,
            },
            skills = skills
        };
    }
}
