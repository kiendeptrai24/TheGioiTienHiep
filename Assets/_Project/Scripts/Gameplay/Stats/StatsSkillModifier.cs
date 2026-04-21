using System;
using System.Collections.Generic;

public class StatsSkillModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var datas = heroData.skillDatas;
        if (datas == null) return;
        base.AddStats(stats, itemData);

        foreach (var data in datas)
        {
            AddPercent(StatType.Health, data.bonusHealth);
            AddPercent(StatType.Mana, data.bonusMana);
            AddPercent(StatType.Spirit, data.bonusSpirit);
            AddPercent(StatType.CritPower, data.physicalDamage);
            AddPercent(StatType.CritChance, data.magicalDamage);
            AddPercent(StatType.PhysicalDamage, data.spiritDamage);
            AddPercent(StatType.MagicalDamage, data.physicalDefense);
            AddPercent(StatType.SpiritDamage, data.magicalDefense);
            AddPercent(StatType.ArmorPenetration, data.spiritDefense);
            AddPercent(StatType.CritDamageReduction, data.critDamage);
            AddPercent(StatType.TrueDamage, data.critRate);
            AddPercent(StatType.PhysicalDefense, data.armorPenetration);
            AddPercent(StatType.MagicalDefense, data.critDamageReduction);
            AddPercent(StatType.SpiritDefense, data.trueDamage);
            AddPercent(StatType.AttackSpeed, data.attackSpeed);
        }
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var datas = heroData.skillDatas;
        if (datas == null) return;
        base.RemoveStats(stats, itemData);

        foreach (var data in datas)
        {
            RemovePercent(StatType.Health, data.bonusHealth);
            RemovePercent(StatType.Mana, data.bonusMana);
            RemovePercent(StatType.Spirit, data.bonusSpirit);
            RemovePercent(StatType.CritPower, data.physicalDamage);
            RemovePercent(StatType.CritChance, data.magicalDamage);
            RemovePercent(StatType.PhysicalDamage, data.spiritDamage);
            RemovePercent(StatType.MagicalDamage, data.physicalDefense);
            RemovePercent(StatType.SpiritDamage, data.magicalDefense);
            RemovePercent(StatType.ArmorPenetration, data.spiritDefense);
            RemovePercent(StatType.CritDamageReduction, data.critDamage);
            RemovePercent(StatType.TrueDamage, data.critRate);
            RemovePercent(StatType.PhysicalDefense, data.armorPenetration);
            RemovePercent(StatType.MagicalDefense, data.critDamageReduction);
            RemovePercent(StatType.SpiritDefense, data.trueDamage);
            RemovePercent(StatType.AttackSpeed, data.attackSpeed);
        }
    }
}