using UnityEngine;

public static class DamageFormula
{
    public static (int damage, bool isCrit, float lifeSteal, float reflect) Calc(in UnitSnapshot atk, in UnitSnapshot def, ref XorShift32 rng)
    {
        if (def.dmgImmunity >= 1f) return (0, false, 0, 0);

        float finalPhysicalDef = Mathf.Max(0, def.physicalDef - def.physicalDef * atk.armorPen);
        float finalPhysical = Mathf.Max(0, atk.physicalDmg - finalPhysicalDef);

        float finalMagicalDef = Mathf.Max(0, def.magicalDef - def.magicalDef * atk.armorPen);
        float finalMagical = Mathf.Max(0, atk.magicalDmg - finalMagicalDef);

        float finalSpiritDef = Mathf.Max(0, def.spiritDef - def.spiritDef * atk.spiritPen);
        float finalSpirit = Mathf.Max(0, atk.spiritDmg - finalSpiritDef);

        float total = finalPhysical + finalMagical + finalSpirit;

        bool isCrit = rng.Next01() < atk.critChance;
        if (isCrit)
            total *= atk.critPower * (1f - def.critReduction);

        total *= (1f - def.penReduction);
        total += atk.trueDmg * (1f - def.trueReduction);
        total *= (1f - def.dmgImmunity);

        int dmg = Mathf.Max(0, Mathf.RoundToInt(total));
        float ls = atk.lifeSteal * total;
        float rf = def.reflect * total;
        return (dmg, isCrit, ls, rf);
    }
}
