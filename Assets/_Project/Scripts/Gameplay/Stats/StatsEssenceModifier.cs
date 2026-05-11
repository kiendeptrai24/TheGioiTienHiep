using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsEssenceModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var data = heroData.essenceData;
        if (data == null) return;
        base.AddStats(stats, itemData);

        int healthPoint = heroData.healthPoint;
        int manaPoint = heroData.manaPoint;
        int spiritPoint = heroData.spiritPoint;

        int physicalDamagePoint = heroData.physicalDamagePoint;
        int magicalDamagePoint = heroData.physicalDamagePoint;
        int spiritDamagePoint = heroData.physicalDamagePoint;

        int physicalDefensePoint = heroData.physicalDefensePoint;
        int magicalDefensePoint = heroData.physicalDefensePoint;
        int spiritDefensePoint = heroData.physicalDefensePoint;

        int movementSpeedPoint = heroData.moveSpeedPoint;
        int spiritRangePoint = heroData.spiritPoint;

        AddValue(StatType.Health, data.healthPoint * healthPoint);
        AddValue(StatType.Mana, data.manaPoint * manaPoint);
        AddValue(StatType.Spirit, data.spiritPoint * spiritPoint);

        AddValue(StatType.PhysicalDamage, data.physicalDamagePoint * physicalDamagePoint);
        AddValue(StatType.MagicalDamage, data.magicalDamagePoint * magicalDamagePoint);
        AddValue(StatType.SpiritDamage, data.spiritDamagePoint * spiritDamagePoint);

        AddValue(StatType.PhysicalDefense, data.physicalDefensePoint * physicalDefensePoint);
        AddValue(StatType.MagicalDefense, data.magicalDefensePoint * magicalDefensePoint);
        AddValue(StatType.SpiritDefense, data.spiritDefensePoint * spiritDefensePoint);

        AddValue(StatType.MovementSpeed, data.movementSpeedPoint * movementSpeedPoint);
        AddValue(StatType.SpiritRange, data.spiritRangePoint * spiritRangePoint);
        AddValue(StatType.CounterPercentage, data.counterPercentage * data.counterPercentage);
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var data = heroData.essenceData;
        if (data == null) return;
        base.RemoveStats(stats, itemData);

        int healthPoint = heroData.healthPoint;
        int manaPoint = heroData.manaPoint;
        int spiritPoint = heroData.spiritPoint;

        int physicalDamagePoint = heroData.physicalDamagePoint;
        int magicalDamagePoint = heroData.physicalDamagePoint;
        int spiritDamagePoint = heroData.physicalDamagePoint;

        int physicalDefensePoint = heroData.physicalDefensePoint;
        int magicalDefensePoint = heroData.physicalDefensePoint;
        int spiritDefensePoint = heroData.physicalDefensePoint;

        int movementSpeedPoint = heroData.moveSpeedPoint;
        int spiritRangePoint = heroData.spiritPoint;

        RemoveValue(StatType.Health, data.healthPoint * healthPoint);
        RemoveValue(StatType.Mana, data.manaPoint * manaPoint);
        RemoveValue(StatType.Spirit, data.spiritPoint * spiritPoint);

        RemoveValue(StatType.PhysicalDamage, data.physicalDamagePoint * physicalDamagePoint);
        RemoveValue(StatType.MagicalDamage, data.magicalDamagePoint * magicalDamagePoint);
        RemoveValue(StatType.SpiritDamage, data.spiritDamagePoint * spiritDamagePoint);

        RemoveValue(StatType.PhysicalDefense, data.physicalDefensePoint * physicalDefensePoint);
        RemoveValue(StatType.MagicalDefense, data.magicalDefensePoint * magicalDefensePoint);
        RemoveValue(StatType.SpiritDefense, data.spiritDefensePoint * spiritDefensePoint);

        RemoveValue(StatType.MovementSpeed, data.movementSpeedPoint * movementSpeedPoint);
        RemoveValue(StatType.SpiritRange, data.spiritRangePoint * spiritRangePoint);
        RemoveValue(StatType.CounterPercentage, data.counterPercentage * data.counterPercentage);
    }
}