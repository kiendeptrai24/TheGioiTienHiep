using System;
using System.Collections.Generic;

public class StatsPointModifier : StatsModifierBase
{
    public override void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var data = itemData as HeroData;

        if (data == null) return;
        base.AddStats(stats, itemData);

        AddValue(StatType.Health, StatPointConfig.GetDamage(data.healthPoint));
        AddValue(StatType.Mana, StatPointConfig.GetDamage(data.manaPoint));
        AddValue(StatType.Spirit, StatPointConfig.GetDamage(data.spiritPoint));

        AddValue(StatType.PhysicalDamage, StatPointConfig.GetDefense(data.physicalDamagePoint));
        AddValue(StatType.MagicalDamage, StatPointConfig.GetDefense(data.magicalDamagePoint));
        AddValue(StatType.SpiritDamage, StatPointConfig.GetDefense(data.spiritDamagePoint));

        AddValue(StatType.PhysicalDefense, StatPointConfig.GetHealth(data.physicalDefensePoint));
        AddValue(StatType.MagicalDefense, StatPointConfig.GetMana(data.magicalDefensePoint));
        AddValue(StatType.SpiritDefense, StatPointConfig.GetSpirit(data.spiritDefensePoint));

        AddValue(StatType.MovementSpeed, StatPointConfig.GetMoveSpeed(data.moveSpeedPoint));
        AddValue(StatType.SpiritRange, StatPointConfig.GetSpiritRange(data.spiritPoint));

    }
    public override void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        if (itemData == null) return;
        var data = itemData as HeroData;
        if (data == null) return;
        base.RemoveStats(stats, itemData);

        RemoveValue(StatType.Health, StatPointConfig.GetDamage(data.healthPoint));
        RemoveValue(StatType.Mana, StatPointConfig.GetDamage(data.manaPoint));
        RemoveValue(StatType.Spirit, StatPointConfig.GetDamage(data.spiritPoint));

        RemoveValue(StatType.PhysicalDamage, StatPointConfig.GetDefense(data.physicalDamagePoint));
        RemoveValue(StatType.MagicalDamage, StatPointConfig.GetDefense(data.magicalDamagePoint));
        RemoveValue(StatType.SpiritDamage, StatPointConfig.GetDefense(data.spiritDamagePoint));

        RemoveValue(StatType.PhysicalDefense, StatPointConfig.GetHealth(data.physicalDefensePoint));
        RemoveValue(StatType.MagicalDefense, StatPointConfig.GetMana(data.magicalDefensePoint));
        RemoveValue(StatType.SpiritDefense, StatPointConfig.GetSpirit(data.spiritDefensePoint));

        RemoveValue(StatType.MovementSpeed, StatPointConfig.GetMoveSpeed(data.moveSpeedPoint));
        RemoveValue(StatType.SpiritRange, StatPointConfig.GetSpiritRange(data.spiritPoint));


    }
    public static class StatPointConfig
    {
        public const float DAMAGE_PER_POINT = 2f;
        public const float DEFENSE_PER_POINT = 1.5f;
        public const float HEALTH_PER_POINT = 10f;
        public const float MANA_PER_POINT = 5f;
        public const float SPIRIT_PER_POINT = 3f;
        public const float MOVE_SPEED_PER_POINT = 0.2f;
        public const float SPIRIT_RANGE_PER_POINT = 0.5f;

        public static float GetDamage(int point) => point * DAMAGE_PER_POINT;
        public static float GetDefense(int point) => point * DEFENSE_PER_POINT;
        public static float GetHealth(int point) => point * HEALTH_PER_POINT;
        public static float GetMana(int point) => point * MANA_PER_POINT;
        public static float GetSpirit(int point) => point * SPIRIT_PER_POINT;
        public static float GetMoveSpeed(int point) => point * MOVE_SPEED_PER_POINT;
        public static float GetSpiritRange(int point) => point * SPIRIT_RANGE_PER_POINT;
    }
}