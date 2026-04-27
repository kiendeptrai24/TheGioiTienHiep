using System;
using System.Collections.Generic;

public class StatsRaceModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var race = itemData as RaceData;
        var heroData = itemData as HeroData;
        if (race == null) return;
        base.AddStats(stats, itemData);
        AddPercent(StatType.Health, race.healthPoint);
        AddPercent(StatType.Mana, race.manaPoint);
        AddPercent(StatType.Spirit, race.spiritPoint);
        AddPercent(StatType.PhysicalDamage, race.physicalDamage);
        AddPercent(StatType.MagicalDamage, race.magicalDamage);
        AddPercent(StatType.SpiritDamage, race.spiritDamage);
        AddPercent(StatType.PhysicalDefense, race.physicalDefense);
        AddPercent(StatType.MagicalDefense, race.magicalDefense);
        AddPercent(StatType.SpiritDefense, race.spiritDefense);
        AddPercent(StatType.MovementSpeed, race.movementSpeedPoint);
        AddPercent(StatType.SpiritRange, race.spiritRangePoint);
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var race = itemData as RaceData;
        var heroData = itemData as HeroData;
        if (race == null) return;
        if (heroData == null) return;
        base.RemoveStats(stats, itemData);

        RemovePercent(StatType.Health, race.healthPoint);
        RemovePercent(StatType.Mana, race.manaPoint);
        RemovePercent(StatType.Spirit, race.spiritPoint);
        RemovePercent(StatType.PhysicalDamage, race.physicalDamage);
        RemovePercent(StatType.MagicalDamage, race.magicalDamage);
        RemovePercent(StatType.SpiritDamage, race.spiritDamage);
        RemovePercent(StatType.PhysicalDefense, race.physicalDefense);
        RemovePercent(StatType.MagicalDefense, race.magicalDefense);
        RemovePercent(StatType.SpiritDefense, race.spiritDefense);
        RemovePercent(StatType.MovementSpeed, race.movementSpeedPoint);
        RemovePercent(StatType.SpiritRange, race.spiritRangePoint);
    }
}