using System;
using System.Collections.Generic;

public class StatsCultivationPathModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var data = heroData.statsCultivationPathData;
        if (data == null) return;
        base.AddStats(stats, itemData);

        AddValue(StatType.Health, data.maxHealth);
        AddValue(StatType.Mana, data.maxMana);
        AddValue(StatType.Spirit, data.maxSpirit);
        AddValue(StatType.PhysicalDamage, data.physicalDamage);
        AddValue(StatType.MagicalDamage, data.magicalDamage);
        AddValue(StatType.SpiritDamage, data.spiritDamage);
        AddValue(StatType.PhysicalDefense, data.physicalDefense);
        AddValue(StatType.MagicalDefense, data.magicalDefense);
        AddValue(StatType.SpiritDefense, data.spiritDefense);
        AddValue(StatType.MovementSpeed, data.movementSpeed);
        AddValue(StatType.SpiritRange, data.spiritRange);
        AddValue(StatType.CounterPercentage, data.counterPercentage);
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var data = heroData.statsCultivationPathData;
        if (data == null) return;
        base.RemoveStats(stats, itemData);

        RemoveValue(StatType.Health, data.maxHealth);
        RemoveValue(StatType.Mana, data.maxMana);
        RemoveValue(StatType.Spirit, data.maxSpirit);
        RemoveValue(StatType.PhysicalDamage, data.physicalDamage);
        RemoveValue(StatType.MagicalDamage, data.magicalDamage);
        RemoveValue(StatType.SpiritDamage, data.spiritDamage);
        RemoveValue(StatType.PhysicalDefense, data.physicalDefense);
        RemoveValue(StatType.MagicalDefense, data.magicalDefense);
        RemoveValue(StatType.SpiritDefense, data.spiritDefense);
        RemoveValue(StatType.MovementSpeed, data.movementSpeed);
        RemoveValue(StatType.SpiritRange, data.spiritRange);
        RemoveValue(StatType.CounterPercentage, data.counterPercentage);
    }
}