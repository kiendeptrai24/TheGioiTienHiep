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
        int maxHealth = heroData.healthPoint;
        int maxMana = heroData.manaPoint;
        int maxSpirit = heroData.spiritPoint;

        int physicalDamage = heroData.physicalDamagePoint;
        int magicalDamage = heroData.magicalDamagePoint;
        int spiritDamage = heroData.spiritDamagePoint;

        int physicalDefense = heroData.physicalDefensePoint;
        int magicalDefense = heroData.magicalDefensePoint;
        int spiritDefense = heroData.spiritDefensePoint;

        int movementSpeed = heroData.moveSpeedPoint;
        int spiritRange = heroData.spiritPoint;


        AddValue(StatType.Health, data.maxHealth * maxHealth);
        AddValue(StatType.Mana, data.maxMana * maxMana);
        AddValue(StatType.Spirit, data.maxSpirit * maxSpirit);

        AddValue(StatType.PhysicalDamage, data.physicalDamage * physicalDamage);
        AddValue(StatType.MagicalDamage, data.magicalDamage * magicalDamage);
        AddValue(StatType.SpiritDamage, data.spiritDamage * spiritDamage);

        AddValue(StatType.PhysicalDefense, data.physicalDefense * physicalDefense);
        AddValue(StatType.MagicalDefense, data.magicalDefense * magicalDefense);
        AddValue(StatType.SpiritDefense, data.spiritDefense * spiritDefense);

        AddValue(StatType.MovementSpeed, data.movementSpeed * movementSpeed);
        AddValue(StatType.SpiritRange, data.spiritRange * spiritRange);
        AddValue(StatType.CounterPercentage, data.counterPercentage * data.counterPercentage);
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var data = heroData.statsCultivationPathData;
        if (data == null) return;
        base.RemoveStats(stats, itemData);
        int maxHealth = heroData.healthPoint == 0 ? 1 : heroData.healthPoint;
        int maxMana = heroData.manaPoint == 0 ? 1 : heroData.manaPoint;
        int maxSpirit = heroData.spiritPoint == 0 ? 1 : heroData.spiritPoint;

        int physicalDamage = heroData.physicalDamagePoint == 0 ? 1 : heroData.physicalDamagePoint;
        int magicalDamage = heroData.magicalDamagePoint == 0 ? 1 : heroData.magicalDamagePoint;
        int spiritDamage = heroData.spiritDamagePoint == 0 ? 1 : heroData.spiritDamagePoint;

        int physicalDefense = heroData.physicalDefensePoint == 0 ? 1 : heroData.physicalDefensePoint;
        int magicalDefense = heroData.magicalDefensePoint == 0 ? 1 : heroData.magicalDefensePoint;
        int spiritDefense = heroData.spiritDefensePoint == 0 ? 1 : heroData.spiritDefensePoint;

        int movementSpeed = heroData.moveSpeedPoint == 0 ? 1 : heroData.moveSpeedPoint;
        int spiritRange = heroData.spiritPoint == 0 ? 1 : heroData.spiritPoint;

        RemoveValue(StatType.Health, data.maxHealth * maxHealth);
        RemoveValue(StatType.Mana, data.maxMana * maxMana);
        RemoveValue(StatType.Spirit, data.maxSpirit * maxSpirit);

        RemoveValue(StatType.PhysicalDamage, data.physicalDamage * physicalDamage);
        RemoveValue(StatType.MagicalDamage, data.magicalDamage * magicalDamage);
        RemoveValue(StatType.SpiritDamage, data.spiritDamage * spiritDamage);

        RemoveValue(StatType.PhysicalDefense, data.physicalDefense * physicalDefense);
        RemoveValue(StatType.MagicalDefense, data.magicalDefense * magicalDefense);
        RemoveValue(StatType.SpiritDefense, data.spiritDefense * spiritDefense);

        RemoveValue(StatType.MovementSpeed, data.movementSpeed * movementSpeed);
        RemoveValue(StatType.SpiritRange, data.spiritRange * spiritRange);
        RemoveValue(StatType.CounterPercentage, data.counterPercentage);
    }
}