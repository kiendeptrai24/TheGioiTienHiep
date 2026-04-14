using System;
using System.Collections.Generic;

public class StatsRaceModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var race = itemData as StatsRaceData;
        var heroData = itemData as HeroData;
        if (race == null) return;
        base.AddStats(stats, itemData);
        AddPercent(StatType.Health, race.maxHealth);
        AddPercent(StatType.Mana, race.maxMana);
        AddPercent(StatType.Spirit, race.maxSpirit);
        AddPercent(StatType.PhysicalDamage, race.physicalDamage);
        AddPercent(StatType.MagicalDamage, race.magicalDamage);
        AddPercent(StatType.SpiritDamage, race.spiritDamage);
        AddPercent(StatType.PhysicalDefense, race.physicalDefense);
        AddPercent(StatType.MagicalDefense, race.magicalDefense);
        AddPercent(StatType.SpiritDefense, race.spiritDefense);
        AddPercent(StatType.MovementSpeed, race.movementSpeed);
        AddPercent(StatType.SpiritRange, race.spiritRange);
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var race = itemData as StatsRaceData;
        var heroData = itemData as HeroData;
        if (race == null) return;
        if (heroData == null) return;
        base.RemoveStats(stats, itemData);

        RemovePercent(StatType.Health, race.maxHealth);
        RemovePercent(StatType.Mana, race.maxMana);
        RemovePercent(StatType.Spirit, race.maxSpirit);
        RemovePercent(StatType.PhysicalDamage, race.physicalDamage);
        RemovePercent(StatType.MagicalDamage, race.magicalDamage);
        RemovePercent(StatType.SpiritDamage, race.spiritDamage);
        RemovePercent(StatType.PhysicalDefense, race.physicalDefense);
        RemovePercent(StatType.MagicalDefense, race.magicalDefense);
        RemovePercent(StatType.SpiritDefense, race.spiritDefense);
        RemovePercent(StatType.MovementSpeed, race.movementSpeed);
        RemovePercent(StatType.SpiritRange, race.spiritRange);
    }
}