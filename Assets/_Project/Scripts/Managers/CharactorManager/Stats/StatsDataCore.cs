using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsDataCore
{
    public ItemData heroData;

    private Dictionary<StatType, Stat> Stats = new();

    private List<IStatsModifier> statsModifiers;

    public StatsDataCore(ItemData heroData)
    {
        this.heroData = heroData;
    }
    
    #region Base Stats
    public int Health => GetStatValue(StatType.Health);
    public int Mana => GetStatValue(StatType.Mana);
    public int Spirit => GetStatValue(StatType.Spirit);
    #endregion
    #region Damage
    public int PhysicalDamage => GetStatValue(StatType.PhysicalDamage);
    public int MagicalDamage => GetStatValue(StatType.MagicalDamage);
    public int SpiritDamage => GetStatValue(StatType.SpiritDamage);

    public float TrueDamage => GetStatValue(StatType.TrueDamage);
    public float ArmorPenetration => GetStatValue(StatType.ArmorPenetration);
    public float SpiritPenetration => GetStatValue(StatType.SpiritPenetration);
    public float LifeSteal => GetStatValue(StatType.LifeSteal);
    #endregion

    #region Defense & Damage Reduction
    public int PhysicalDefense => GetStatValue(StatType.PhysicalDefense);
    public int MagicalDefense => GetStatValue(StatType.MagicalDefense);
    public int SpiritDefense => GetStatValue(StatType.SpiritDefense);

    public float ReflectDamage => GetStatValue(StatType.ReflectDamage);

    public float CritDamageReduction => GetStatValue(StatType.CritDamageReduction);
    public float PenetrationDamageReduction => GetStatValue(StatType.PenetrationDamageReduction);
    public float TrueDamageReduction => GetStatValue(StatType.TrueDamageReduction);
    #endregion

    #region Speed
    public int MovementSpeed => GetStatValue(StatType.MovementSpeed);
    public int AttackSpeed => GetStatValue(StatType.AttackSpeed);
    #endregion

    #region Range
    public int AttackRange => GetStatValue(StatType.AttackRange);
    public int SpiritRange => GetStatValue(StatType.SpiritRange);

    public float DamageImmunity => GetStatValue(StatType.DamageImmunity);
    #endregion
    public Dictionary<StatType, Stat> GetStats() => Stats;

    public int GetStatValue(StatType type)
    {
        if (Stats.TryGetValue(type, out Stat stat))
        {
            return Mathf.RoundToInt(stat.GetValue());
        }

        return 0;
    }
    public Stat GetStat(StatType type) => Stats[type];

    public void SetStatsModifier(List<IStatsModifier> statsModifiers = null)
    {
        if (statsModifiers == null)
        {
            statsModifiers = new();
        }
        else
        {
            this.statsModifiers = statsModifiers;
        }
    }

    public void ResetStats()
    {
        Stats.Clear();

        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            Stats.Add(type, new Stat(type, 0));
        }
    }

    private void Setup()
    {
        ResetStats();

        foreach (var modifier in statsModifiers)
        {
            modifier.AddStats(Stats, heroData);
        }

    }
    public void SetUp(ItemData heroData)
    {
        this.heroData = heroData;
        if (statsModifiers == null) SetStatsModifier();
        Setup();
    }
}