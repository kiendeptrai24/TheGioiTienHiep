using System.Collections.Generic;

public class StatsModifier
{
    public void AddStatsRaceData(Dictionary<StatType, Stat> stats, StatsRaceData data)
    {
        if (data == null) return;
        stats.TryGetValue(StatType.Health, out Stat healthStat);
        stats.TryGetValue(StatType.Mana, out Stat manaStat);
        stats.TryGetValue(StatType.Spirit, out Stat spiritStat);
        stats.TryGetValue(StatType.PhysicalDamage, out Stat physicalDamageStat);
        stats.TryGetValue(StatType.MagicalDamage, out Stat magicalDamageStat);
        stats.TryGetValue(StatType.SpiritDamage, out Stat spiritDamageStat);
        stats.TryGetValue(StatType.PhysicalDefense, out Stat physicalDefenseStat);
        stats.TryGetValue(StatType.MagicalDefense, out Stat magicalDefenseStat);
        stats.TryGetValue(StatType.SpiritDefense, out Stat spiritDefenseStat);
        stats.TryGetValue(StatType.MovementSpeed, out Stat movementSpeedStat);
        stats.TryGetValue(StatType.SpiritRange, out Stat spiritRangeStat);

        healthStat.AddModifierPercent(data.maxHealth);
        manaStat.AddModifierPercent(data.maxMana);
        spiritStat.AddModifierPercent(data.maxSpirit);
        physicalDamageStat.AddModifierPercent(data.physicalDamage);
        magicalDamageStat.AddModifierPercent(data.magicalDamage);
        spiritDamageStat.AddModifierPercent(data.spiritDamage);
        physicalDefenseStat.AddModifierPercent(data.physicalDefense);
        magicalDefenseStat.AddModifierPercent(data.magicalDefense);
        spiritDefenseStat.AddModifierPercent(data.spiritDefense);
        movementSpeedStat.AddModifierPercent(data.movementSpeed);
        spiritRangeStat.AddModifierPercent(data.spiritRange);
    }
    public void AddStatsRealmData(Dictionary<StatType, Stat> stats, StatsRealmData data)
    {
        if (data == null) return;
        stats.TryGetValue(StatType.Health, out Stat healthStat);
        stats.TryGetValue(StatType.Mana, out Stat manaStat);
        stats.TryGetValue(StatType.Spirit, out Stat spiritStat);

        stats.TryGetValue(StatType.CritPower, out Stat critRateStat);
        stats.TryGetValue(StatType.CritChance, out Stat critDamageStat);

        stats.TryGetValue(StatType.ArmorPenetration, out Stat armorPenetrationStat);

        stats.TryGetValue(StatType.PhysicalDamage, out Stat physicalDamageStat);
        stats.TryGetValue(StatType.MagicalDamage, out Stat magicalDamageStat);
        stats.TryGetValue(StatType.SpiritDamage, out Stat spiritDamageStat);

        stats.TryGetValue(StatType.PhysicalDefense, out Stat physicalDefenseStat);
        stats.TryGetValue(StatType.MagicalDefense, out Stat magicalDefenseStat);
        stats.TryGetValue(StatType.SpiritDefense, out Stat spiritDefenseStat);

        stats.TryGetValue(StatType.Potential, out Stat potentialStat);
        stats.TryGetValue(StatType.SkillPoints, out Stat skillPointsStat);

        stats.TryGetValue(StatType.MovementSpeed, out Stat movementSpeedStat);
        stats.TryGetValue(StatType.AttackSpeed, out Stat attackSpeedStat);

        stats.TryGetValue(StatType.CastSpeed, out Stat castSpeedStat);
        stats.TryGetValue(StatType.CombatPower, out Stat combatPowerStat);
        stats.TryGetValue(StatType.SpiritRange, out Stat spiritRangeStat);
        stats.TryGetValue(StatType.Evasion, out Stat evasionStat);

        healthStat.AddModifier(data.maxHealth);
        manaStat.AddModifier(data.maxMana);
        spiritStat.AddModifier(data.maxSpirit);

        critRateStat.AddModifier(data.critRate);
        critDamageStat.AddModifier(data.critDamage);

        armorPenetrationStat.AddModifier(data.armorPenetration);

        physicalDamageStat.AddModifier(data.physicalDamage);
        magicalDamageStat.AddModifier(data.magicalDamage);
        spiritDamageStat.AddModifier(data.spiritDamage);

        physicalDefenseStat.AddModifier(data.physicalDefense);
        magicalDefenseStat.AddModifier(data.magicalDefense);
        spiritDefenseStat.AddModifier(data.spiritDefense);

        potentialStat.AddModifier(data.potential);
        skillPointsStat.AddModifier(data.skillPoints);

        movementSpeedStat.AddModifier(data.movementSpeed);
        attackSpeedStat.AddModifier(data.attackSpeed);

        castSpeedStat.AddModifier(data.castSpeed);
        combatPowerStat.AddModifier(data.combatPower);
        spiritRangeStat.AddModifier(data.spiritRange);

        evasionStat.AddModifier(data.evasion);
    }
    public void AddStatsCultivationPathData(Dictionary<StatType, Stat> stats, StatsCultivationPathData data)
    {
        if (data == null) return;
        stats.TryGetValue(StatType.Health, out Stat healthStat);
        stats.TryGetValue(StatType.Mana, out Stat manaStat);
        stats.TryGetValue(StatType.Spirit, out Stat spiritStat);

        stats.TryGetValue(StatType.PhysicalDamage, out Stat physicalDamageStat);
        stats.TryGetValue(StatType.MagicalDamage, out Stat magicalDamageStat);
        stats.TryGetValue(StatType.SpiritDamage, out Stat spiritDamageStat);

        stats.TryGetValue(StatType.PhysicalDefense, out Stat physicalDefenseStat);
        stats.TryGetValue(StatType.MagicalDefense, out Stat magicalDefenseStat);
        stats.TryGetValue(StatType.SpiritDefense, out Stat spiritDefenseStat);

        stats.TryGetValue(StatType.MovementSpeed, out Stat movementSpeedStat);
        stats.TryGetValue(StatType.SpiritRange, out Stat spiritRangeStat);

        stats.TryGetValue(StatType.CounterPercentage, out Stat counterPercentageStat);

        healthStat.AddModifier(data.maxHealth);
        manaStat.AddModifier(data.maxMana);
        spiritStat.AddModifier(data.maxSpirit);

        physicalDamageStat.AddModifier(data.physicalDamage);
        magicalDamageStat.AddModifier(data.magicalDamage);
        spiritDamageStat.AddModifier(data.spiritDamage);

        physicalDefenseStat.AddModifier(data.physicalDefense);
        magicalDefenseStat.AddModifier(data.magicalDefense);
        spiritDefenseStat.AddModifier(data.spiritDefense);

        movementSpeedStat.AddModifier(data.movementSpeed);
        spiritRangeStat.AddModifier(data.spiritRange);

        counterPercentageStat.AddModifier(data.counterPercentage);

    }
    public void AddStatsHeroData(Dictionary<StatType, Stat> stats, ItemData data)
    {
        HeroData heroData = data as HeroData;
        if (heroData == null) return;
        stats.TryGetValue(StatType.Health, out Stat healthStat);
        stats.TryGetValue(StatType.Mana, out Stat manaStat);
        stats.TryGetValue(StatType.Spirit, out Stat spiritStat);

        stats.TryGetValue(StatType.PhysicalDamage, out Stat physicalDamageStat);
        stats.TryGetValue(StatType.MagicalDamage, out Stat magicalDamageStat);
        stats.TryGetValue(StatType.SpiritDamage, out Stat spiritDamageStat);

        stats.TryGetValue(StatType.PhysicalDefense, out Stat physicalDefenseStat);
        stats.TryGetValue(StatType.MagicalDefense, out Stat magicalDefenseStat);
        stats.TryGetValue(StatType.SpiritDefense, out Stat spiritDefenseStat);

        stats.TryGetValue(StatType.MovementSpeed, out Stat movementSpeedStat);
        stats.TryGetValue(StatType.AttackSpeed, out Stat attackSpeedStat);

        healthStat.AddModifierPercent(heroData.health);
        manaStat.AddModifierPercent(heroData.mana);
        spiritStat.AddModifierPercent(heroData.spirit);

        physicalDamageStat.AddModifierPercent(heroData.physicalDamage);
        magicalDamageStat.AddModifierPercent(heroData.magicalDamage);
        spiritDamageStat.AddModifierPercent(heroData.spiritDamage);

        physicalDefenseStat.AddModifierPercent(heroData.physicalDefense);
        magicalDefenseStat.AddModifierPercent(heroData.magicalDefense);
        spiritDefenseStat.AddModifierPercent(heroData.spiritDefense);

        movementSpeedStat.AddModifier(heroData.moveSpeed);
        attackSpeedStat.AddModifier(heroData.attackRange);
    }
    public void AddStatsTechniqueData(Dictionary<StatType, Stat> stats, List<TechniqueData> datas)
    {
        if (datas == null) return;
        stats.TryGetValue(StatType.Health, out Stat healthStat);
        stats.TryGetValue(StatType.Mana, out Stat manaStat);
        stats.TryGetValue(StatType.Spirit, out Stat spiritStat);

        stats.TryGetValue(StatType.CritPower, out Stat critPower);
        stats.TryGetValue(StatType.CritChance, out Stat critChance);
        

        stats.TryGetValue(StatType.PhysicalDamage, out Stat physicalDamageStat);
        stats.TryGetValue(StatType.MagicalDamage, out Stat magicalDamageStat);
        stats.TryGetValue(StatType.SpiritDamage, out Stat spiritDamageStat);

        stats.TryGetValue(StatType.ArmorPenetration, out Stat armorPenetrationStat);
        stats.TryGetValue(StatType.CritDamageReduction, out Stat critDamageReduction);
        stats.TryGetValue(StatType.TrueDamage, out Stat trueDamage);

        stats.TryGetValue(StatType.PhysicalDefense, out Stat physicalDefenseStat);
        stats.TryGetValue(StatType.MagicalDefense, out Stat magicalDefenseStat);
        stats.TryGetValue(StatType.SpiritDefense, out Stat spiritDefenseStat);

        stats.TryGetValue(StatType.AttackSpeed, out Stat attackSpeedStat);

       foreach (var item in datas)
       {
            healthStat.AddModifierPercent(item.bonusHealth);
            manaStat.AddModifierPercent(item.bonusMana);
            spiritStat.AddModifierPercent(item.bonusSpirit);
            physicalDamageStat.AddModifierPercent(item.physicalDamage);
            magicalDamageStat.AddModifierPercent(item.magicalDamage);
            spiritDamageStat.AddModifierPercent(item.spiritDamage);
            physicalDefenseStat.AddModifierPercent(item.physicalDefense);
            magicalDefenseStat.AddModifierPercent(item.magicalDefense);
            spiritDefenseStat.AddModifierPercent(item.spiritDefense);

            critPower.AddModifierPercent(item.critDamage);
            critChance.AddModifierPercent(item.critRate);

            armorPenetrationStat.AddModifierPercent(item.armorPenetration);
            critDamageReduction.AddModifierPercent(item.critDamageReduction);
            trueDamage.AddModifierPercent(item.trueDamage);

            attackSpeedStat.AddModifier(item.attackSpeed);
       }


    }    
}