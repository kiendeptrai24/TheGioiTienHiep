using UnityEngine;

public static class SnapshotMapper
{
    public static UnitSnapshot FromStats(StatsData s, int uid, TeamId team)
    {
        int hp = s.Health;
        return new UnitSnapshot
        {
            uid = uid,
            team = team,
            hpMax = hp,
            hp = hp,

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
            critPower  = 2f + s.GetStatValue(StatType.CritPower) / 100f,
            critReduction = s.CritDamageReduction,

            penReduction = s.PenetrationDamageReduction,
            trueReduction = s.TrueDamageReduction,
            dmgImmunity = s.DamageImmunity,

            lifeSteal = s.LifeSteal,
            reflect = s.ReflectDamage,

            attackSpeed = Mathf.Max(1, s.AttackSpeed),
        };
    }
}
