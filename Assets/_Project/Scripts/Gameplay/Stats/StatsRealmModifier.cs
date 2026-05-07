using System;
using System.Collections.Generic;

public class StatsRealmModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var data = heroData.realmData;
        if (data == null) return;
        base.AddStats(stats, itemData);

        AddValue(StatType.Health, data.health);
        AddValue(StatType.Mana, data.mana);
        AddValue(StatType.Spirit, data.spirit);
        AddValue(StatType.CritChance, data.critRate);
        AddValue(StatType.CritPower, data.critDamage);
        AddValue(StatType.ArmorPenetration, data.armorPenetration);
        AddValue(StatType.PhysicalDamage, data.physicalDamage);
        AddValue(StatType.MagicalDamage, data.magicalDamage);
        AddValue(StatType.SpiritDamage, data.spiritDamage);
        AddValue(StatType.PhysicalDefense, data.physicalDefense);
        AddValue(StatType.MagicalDefense, data.magicalDefense);
        AddValue(StatType.SpiritDefense, data.spiritDefense);
        AddValue(StatType.MovementSpeed, data.movementSpeed);
        AddValue(StatType.AttackSpeed, data.attackSpeed);
        AddValue(StatType.CastSpeed, data.castSpeed);
        AddValue(StatType.CombatPower, data.combatPower);
        AddValue(StatType.SpiritRange, data.spiritRange);
        AddValue(StatType.Evasion, data.evasion);
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var data = heroData.realmData;
        if (data == null) return;
        base.RemoveStats(stats, itemData);

        RemoveValue(StatType.Health, data.health);
        RemoveValue(StatType.Mana, data.mana);
        RemoveValue(StatType.Spirit, data.spirit);
        RemoveValue(StatType.CritChance, data.critRate);
        RemoveValue(StatType.CritPower, data.critDamage);
        RemoveValue(StatType.ArmorPenetration, data.armorPenetration);
        RemoveValue(StatType.PhysicalDamage, data.physicalDamage);
        RemoveValue(StatType.MagicalDamage, data.magicalDamage);
        RemoveValue(StatType.SpiritDamage, data.spiritDamage);
        RemoveValue(StatType.PhysicalDefense, data.physicalDefense);
        RemoveValue(StatType.MagicalDefense, data.magicalDefense);
        RemoveValue(StatType.SpiritDefense, data.spiritDefense);
        RemoveValue(StatType.MovementSpeed, data.movementSpeed);
        RemoveValue(StatType.AttackSpeed, data.attackSpeed);
        RemoveValue(StatType.CastSpeed, data.castSpeed);
        RemoveValue(StatType.CombatPower, data.combatPower);
        RemoveValue(StatType.SpiritRange, data.spiritRange);
        RemoveValue(StatType.Evasion, data.evasion);
    }
}