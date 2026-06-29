using System;
using System.Collections.Generic;

public class StatsEquipmentModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var datas = heroData.equipmentDatas;
        if (datas == null) return;
        base.AddStats(stats, itemData);
        foreach (var data in datas)
        {

            AddPercent(StatType.Health, data.health);
            AddPercent(StatType.Mana, data.mana);
            AddPercent(StatType.Spirit, data.spirit);

            AddPercent(StatType.PhysicalDamage, data.physicalDamage);
            AddPercent(StatType.MagicalDamage, data.magicalDamage);
            AddPercent(StatType.SpiritDamage, data.spiritDamage);

            AddPercent(StatType.PhysicalDefense, data.physicalDefense);
            AddPercent(StatType.MagicalDefense, data.magicalDefense);
            AddPercent(StatType.SpiritDefense, data.spiritDefense);
        }
    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var heroData = itemData as HeroData;
        if (heroData == null) return;
        var datas = heroData.equipmentDatas;
        if (datas == null) return;
        base.RemoveStats(stats, itemData);
        foreach (var data in datas)
        {
            RemovePercent(StatType.Health, data.health);
            RemovePercent(StatType.Mana, data.mana);
            RemovePercent(StatType.Spirit, data.spirit);

            RemovePercent(StatType.PhysicalDamage, data.physicalDamage);
            RemovePercent(StatType.MagicalDamage, data.magicalDamage);
            RemovePercent(StatType.SpiritDamage, data.spiritDamage);

            RemovePercent(StatType.PhysicalDefense, data.physicalDefense);
            RemovePercent(StatType.MagicalDefense, data.magicalDefense);
            RemovePercent(StatType.SpiritDefense, data.spiritDefense);
        }
    }
}