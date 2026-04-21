using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StatsCharacterModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var data = itemData as HeroData;
        if (data == null) return;
        base.AddStats(stats, itemData);

        AddValue(StatType.Health, data.health);
        AddValue(StatType.Mana, data.mana);
        AddValue(StatType.Spirit, data.spirit);

        AddValue(StatType.PhysicalDamage, data.physicalDamage);
        AddValue(StatType.MagicalDamage, data.magicalDamage);
        AddValue(StatType.SpiritDamage, data.spiritDamage);

        AddValue(StatType.PhysicalDefense, data.physicalDefense);
        AddValue(StatType.MagicalDefense, data.magicalDefense);
        AddValue(StatType.SpiritDefense, data.spiritDefense);

        AddValue(StatType.MovementSpeed, data.movementSpeed);
        AddValue(StatType.AttackSpeed, data.attackSpeed);

        AddPercent(StatType.HealthRegen, data.healthRegen);
        AddPercent(StatType.ManaRegen, data.manaRegen);
        AddPercent(StatType.SpiritRegen, data.spiritRegen);

        AddValue(StatType.HealthPoint, data.healthPoint);
        AddValue(StatType.ManaPoint, data.manaPoint);
        AddValue(StatType.SpiritPoint, data.spiritPoint);

        AddValue(StatType.PhicialDamagePoint, data.physicalDamagePoint);
        AddValue(StatType.MagicalDamagePoint, data.magicalDamagePoint);

        AddValue(StatType.MoveSpeedPoint, data.moveSpeedPoint);
        AddValue(StatType.SpiritRangePoint, data.spititRangePoint);

        AddValue(StatType.PotentialPoint, data.potentialPoint);
        AddValue(StatType.SkillPoint, data.skillPoint);

    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var data = itemData as HeroData;
        if (data == null) return;

        RemoveValue(StatType.Health, data.health);
        RemoveValue(StatType.Mana, data.mana);
        RemoveValue(StatType.Spirit, data.spirit);

        RemoveValue(StatType.PhysicalDamage, data.physicalDamage);
        RemoveValue(StatType.MagicalDamage, data.magicalDamage);
        RemoveValue(StatType.SpiritDamage, data.spiritDamage);

        RemoveValue(StatType.PhysicalDefense, data.physicalDefense);
        RemoveValue(StatType.MagicalDefense, data.magicalDefense);
        RemoveValue(StatType.SpiritDefense, data.spiritDefense);

        RemoveValue(StatType.MovementSpeed, data.movementSpeed);
        RemoveValue(StatType.AttackSpeed, data.attackSpeed);

        RemovePercent(StatType.HealthRegen, data.healthRegen);
        RemovePercent(StatType.ManaRegen, data.manaRegen);
        RemovePercent(StatType.SpiritRegen, data.spiritRegen);

        RemoveValue(StatType.HealthPoint, data.healthPoint);
        RemoveValue(StatType.ManaPoint, data.manaPoint);
        RemoveValue(StatType.SpiritPoint, data.spiritPoint);

        RemoveValue(StatType.PhicialDamagePoint, data.physicalDamagePoint);
        RemoveValue(StatType.MagicalDamagePoint, data.magicalDamagePoint);

        RemoveValue(StatType.MoveSpeedPoint, data.moveSpeedPoint);
        RemoveValue(StatType.SpiritRangePoint, data.spititRangePoint);

        RemoveValue(StatType.PotentialPoint, data.potentialPoint);
        RemoveValue(StatType.SkillPoint, data.skillPoint);
    }
}