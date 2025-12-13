using System.Collections.Generic;

public class StatsModifier
{
    public void AddStatsRaceData(Dictionary<StatType, Stat> stats, StatsRaceData data)
    {
        if(data == null) return;
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

        healthStat.AddModifier(data.health);
        manaStat.AddModifier(data.mana);
        spiritStat.AddModifier(data.spirit);
        physicalDamageStat.AddModifier(data.physicalDamage);
        magicalDamageStat.AddModifier(data.magicalDamage);
        spiritDamageStat.AddModifier(data.spiritDamage);
        physicalDefenseStat.AddModifier(data.physicalDefense);
        magicalDefenseStat.AddModifier(data.magicalDefense);
        spiritDefenseStat.AddModifier(data.spiritDefense);
        movementSpeedStat.AddModifier(data.movementSpeed);
        spiritRangeStat.AddModifier(data.spiritRange);
    }
    public void AddStatsRealmData(Dictionary<StatType, Stat> stats, StatsRealmData data)
    {
        if(data == null) return;
        stats.TryGetValue(StatType.Health, out Stat healthStat);
        stats.TryGetValue(StatType.Mana, out Stat manaStat);
        stats.TryGetValue(StatType.Spirit, out Stat spiritStat);

        stats.TryGetValue(StatType.CritChance, out Stat critChanceStat);
        stats.TryGetValue(StatType.CritPower, out Stat critPowerStat);

        stats.TryGetValue(StatType.SpiritPenetration, out Stat spiritPenetrationStat);

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

        healthStat.AddModifier(data.health);
        manaStat.AddModifier(data.mana);
        spiritStat.AddModifier(data.spirit);

        critChanceStat.AddModifier(data.critChance);
        critPowerStat.AddModifier(data.critPower);

        spiritPenetrationStat.AddModifier(data.spiritPenetration);

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
        if(data == null) return;
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

        healthStat.AddModifier(data.health);
        manaStat.AddModifier(data.mana);
        spiritStat.AddModifier(data.spirit);
        
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
}