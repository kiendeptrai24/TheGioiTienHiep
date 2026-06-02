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
        AddValue(StatType.Health, race.healthPoint * heroData.healthPoint);
        AddValue(StatType.Mana, race.manaPoint * heroData.manaPoint);
        AddValue(StatType.Spirit, race.spiritPoint * heroData.spiritPoint);
        AddValue(StatType.PhysicalDamage, race.physicalDamage * heroData.physicalDamagePoint);
        AddValue(StatType.MagicalDamage, race.magicalDamage * heroData.magicalDamagePoint);
        AddValue(StatType.SpiritDamage, race.spiritDamage * heroData.spiritDamagePoint);
        AddValue(StatType.PhysicalDefense, race.physicalDefense * heroData.physicalDefensePoint);
        AddValue(StatType.MagicalDefense, race.magicalDefense * heroData.magicalDefensePoint);
        AddValue(StatType.SpiritDefense, race.spiritDefense * heroData.spiritDefensePoint);
        AddValue(StatType.MovementSpeed, race.movementSpeedPoint * heroData.moveSpeedPoint);
        AddValue(StatType.SpiritRange, race.spiritRangePoint * heroData.spiritRangePoint);
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var race = itemData as RaceData;
        var heroData = itemData as HeroData;
        if (race == null) return;
        if (heroData == null) return;
        base.RemoveStats(stats, itemData);

        RemoveValue(StatType.Health, race.healthPoint * heroData.healthPoint);
        RemoveValue(StatType.Mana, race.manaPoint * heroData.manaPoint);
        RemoveValue(StatType.Spirit, race.spiritPoint * heroData.spiritPoint);
        RemoveValue(StatType.PhysicalDamage, race.physicalDamage * heroData.physicalDamagePoint);
        RemoveValue(StatType.MagicalDamage, race.magicalDamage * heroData.magicalDamagePoint);
        RemoveValue(StatType.SpiritDamage, race.spiritDamage * heroData.spiritDamagePoint);
        RemoveValue(StatType.PhysicalDefense, race.physicalDefense * heroData.physicalDefensePoint);
        RemoveValue(StatType.MagicalDefense, race.magicalDefense * heroData.magicalDefensePoint);
        RemoveValue(StatType.SpiritDefense, race.spiritDefense * heroData.spiritDefensePoint);
        RemoveValue(StatType.MovementSpeed, race.movementSpeedPoint * heroData.moveSpeedPoint);
        RemoveValue(StatType.SpiritRange, race.spiritRangePoint * heroData.spiritRangePoint);
    }
}