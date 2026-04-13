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
        AddPercent(StatType.Health, race.maxHealth * StatPointConfig.GetDamage(heroData.healthPoint));
        AddPercent(StatType.Mana, race.maxMana * StatPointConfig.GetDamage(heroData.manaPoint));
        AddPercent(StatType.Spirit, race.maxSpirit * StatPointConfig.GetDamage(heroData.spiritPoint));
        AddPercent(StatType.PhysicalDamage, race.physicalDamage * StatPointConfig.GetDefense(heroData.physicalDamagePoint));
        AddPercent(StatType.MagicalDamage, race.magicalDamage * StatPointConfig.GetDefense(heroData.magicalDamagePoint));
        AddPercent(StatType.SpiritDamage, race.spiritDamage * StatPointConfig.GetDefense(heroData.spiritDamagePoint));
        AddPercent(StatType.PhysicalDefense, race.physicalDefense * StatPointConfig.GetHealth(heroData.physicalDefensePoint));
        AddPercent(StatType.MagicalDefense, race.magicalDefense * StatPointConfig.GetMana(heroData.magicalDefensePoint));
        AddPercent(StatType.SpiritDefense, race.spiritDefense * StatPointConfig.GetSpirit(heroData.spiritDefensePoint));
        AddPercent(StatType.MovementSpeed, race.movementSpeed * StatPointConfig.GetMoveSpeed(heroData.moveSpeedPoint));
        AddPercent(StatType.SpiritRange, race.spiritRange * StatPointConfig.GetSpiritRange(heroData.spiritPoint));
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var race = itemData as StatsRaceData;
        var heroData = itemData as HeroData;
        if (race == null) return;
        if (heroData == null) return;
        base.RemoveStats(stats, itemData);

        RemovePercent(StatType.Health, race.maxHealth * StatPointConfig.GetDamage(heroData.healthPoint));
        RemovePercent(StatType.Mana, race.maxMana * StatPointConfig.GetDamage(heroData.manaPoint));
        RemovePercent(StatType.Spirit, race.maxSpirit * StatPointConfig.GetDamage(heroData.spiritPoint));
        RemovePercent(StatType.PhysicalDamage, race.physicalDamage * StatPointConfig.GetDefense(heroData.physicalDamagePoint));
        RemovePercent(StatType.MagicalDamage, race.magicalDamage * StatPointConfig.GetDefense(heroData.magicalDamagePoint));
        RemovePercent(StatType.SpiritDamage, race.spiritDamage * StatPointConfig.GetDefense(heroData.spiritDamagePoint));
        RemovePercent(StatType.PhysicalDefense, race.physicalDefense * StatPointConfig.GetHealth(heroData.physicalDefensePoint));
        RemovePercent(StatType.MagicalDefense, race.magicalDefense * StatPointConfig.GetMana(heroData.magicalDefensePoint));
        RemovePercent(StatType.SpiritDefense, race.spiritDefense * StatPointConfig.GetSpirit(heroData.spiritDefensePoint));
        RemovePercent(StatType.MovementSpeed, race.movementSpeed * StatPointConfig.GetMoveSpeed(heroData.moveSpeedPoint));
        RemovePercent(StatType.SpiritRange, race.spiritRange * StatPointConfig.GetSpiritRange(heroData.spiritPoint));
    }
}