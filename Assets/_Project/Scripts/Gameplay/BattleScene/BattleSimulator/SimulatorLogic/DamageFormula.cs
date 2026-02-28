using UnityEngine;


public static class DamageFormula
{
    public static (int damage, bool isCrit, float lifeSteal, float reflect)
    Calc(in UnitSnapshot attacker, in UnitSnapshot defender, ref XorShift32 rng)
    {
        if (defender.dmgImmunity >= 1f) return (0, false, 0, 0);
        // Damage sources
        int physicalDmg = attacker.physicalDmg;
        int magicalDmg = attacker.magicalDmg;
        int spiritDmg = attacker.spiritDmg;
        float trueDmg = attacker.trueDmg;

        // Penetration
        float armorPen = attacker.armorPen;
        float spiritPen = attacker.spiritPen;

        // Defense
        int physicalDef = defender.physicalDef;
        int magicalDef = defender.magicalDef;
        int spiritDef = defender.spiritDef;

        // Damage reduction
        float penReduction = defender.penReduction;
        float trueDmgReduction = defender.trueReduction;

        // Immunity
        float damageImmunity = defender.dmgImmunity;
        if (damageImmunity >= 1f) return (0, false, 0, 0);

        // Calculate damage
        float finalPhysicalDef = Mathf.Max(0, physicalDef - physicalDef * armorPen);
        float finalPhysical = Mathf.Max(0, physicalDmg - finalPhysicalDef);

        float finalMagicalDef = Mathf.Max(0, magicalDef - magicalDef * armorPen);
        float finalMagical = Mathf.Max(0, magicalDmg - finalMagicalDef);

        float finalSpiritDef = Mathf.Max(0, spiritDef - spiritDef * spiritPen);
        float finalSpirit = Mathf.Max(0, spiritDmg - finalSpiritDef);

        float totalDmg = finalPhysical + finalMagical + finalSpirit;

        // Crit
        bool isCrit = rng.Next01() < attacker.critChance;
        if (isCrit)
            totalDmg *= attacker.critPower * (1f - defender.critReduction);

        // Penetration reduction
        totalDmg *= (1f - penReduction);

        // True damage
        totalDmg += trueDmg * (1f - trueDmgReduction);

        // Damage immunity
        totalDmg *= (1f - damageImmunity);

        // LifeSteal
        float lifeSteal = attacker.lifeSteal * totalDmg;

        // Reflect
        float reflect = defender.reflect * totalDmg;

        // Clamp
        int final = Mathf.Max(0, Mathf.RoundToInt(totalDmg));
        return (final, isCrit, lifeSteal, reflect);
    }

    // ====== NEW: cast skill -> apply SkillData bonuses for this hit ======
    public static (int damage, bool isCrit, float lifeSteal, float reflect)
    CalcWithSkill(in UnitSnapshot baseAtk, in UnitSnapshot def, SkillData skill, ref XorShift32 rng)
    {
        var atk = baseAtk;

        if (skill != null)
        {
            // crit rate/dmg bonus
            atk.critChance = Mathf.Clamp01(atk.critChance + skill.critRate / 100f);
            atk.critPower = atk.critPower * (1f + skill.critDamage / 100f);

            // penetration / true dmg / lifesteal
            atk.armorPen = Mathf.Clamp01(atk.armorPen + skill.armorPenetration);
            atk.trueDmg = atk.trueDmg + skill.trueDamage;
            atk.lifeSteal = atk.lifeSteal + skill.lifeSteal;

            // attackSpeed bonus (nếu bạn muốn skill làm ra đòn nhanh hơn)
            atk.attackSpeed = Mathf.Max(1, atk.attackSpeed + Mathf.RoundToInt(skill.attackSpeed));
        }

        return Calc(in atk, in def, ref rng);
    }
}
