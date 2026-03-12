using System;
using System.Collections.Generic;

public class StatsRaceModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var data = itemData as StatsRaceData;
        if (data == null) return;
        base.AddStats(stats, itemData);
        AddPercent(StatType.Health, data.maxHealth);
        AddPercent(StatType.Mana, data.maxMana);
        AddPercent(StatType.Spirit, data.maxSpirit);
        AddPercent(StatType.PhysicalDamage, data.physicalDamage);
        AddPercent(StatType.MagicalDamage, data.magicalDamage);
        AddPercent(StatType.SpiritDamage, data.spiritDamage);
        AddPercent(StatType.PhysicalDefense, data.physicalDefense);
        AddPercent(StatType.MagicalDefense, data.magicalDefense);
        AddPercent(StatType.SpiritDefense, data.spiritDefense);
        AddPercent(StatType.MovementSpeed, data.movementSpeed);
        AddPercent(StatType.SpiritRange, data.spiritRange);
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var data = itemData as StatsRaceData;
        if (data == null) return;
        base.RemoveStats(stats, itemData);

        RemovePercent(StatType.Health, data.maxHealth);
        RemovePercent(StatType.Mana, data.maxMana);
        RemovePercent(StatType.Spirit, data.maxSpirit);
        RemovePercent(StatType.PhysicalDamage, data.physicalDamage);
        RemovePercent(StatType.MagicalDamage, data.magicalDamage);
        RemovePercent(StatType.SpiritDamage, data.spiritDamage);
        RemovePercent(StatType.PhysicalDefense, data.physicalDefense);
        RemovePercent(StatType.MagicalDefense, data.magicalDefense);
        RemovePercent(StatType.SpiritDefense, data.spiritDefense);
        RemovePercent(StatType.MovementSpeed, data.movementSpeed);
        RemovePercent(StatType.SpiritRange, data.spiritRange);
    }
}