

using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsData : TGTHMonoBehaviour
{
    public event Action OnValueChanged;
    public event Action<StatsData> OnStatReady;
    public bool IsReady => stats != null && chamionData != null;
    private StatsDataCore stats;

    public ItemData chamionData;
    public HeroData ChampionData;

    [Header("Preset base stats")]
    public RaceData statsRaceData;
    public EssenceData statsCultivationPathData;
    public RealmData statsRealmData;


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
    public float piritRange;
    public void ResetStats()
    {
        stats.ResetStats();
        StatChange();
    }
    #region Stats Emplementation
    public Dictionary<StatType, Stat> GetStats() => stats.GetStats();
    public int GetStatValue(StatType type) => stats.GetStatValue(type);
    public Stat GetStatType(StatType type) => stats.GetStat(type);
    #endregion

    #region Setup Item Data
    private void Setup()
    {
        stats.SetUp(chamionData);
        StatChange();
        OnStatReady?.Invoke(this);
    }

    public void SetUpItem(ItemData item)
    {
        this.chamionData = item;
        ChampionData = item as HeroData;
        if (stats == null)
        {
            stats = new StatsDataCore(item);
            List<IStatsModifier> statsModifiers = new();
            statsModifiers.Add(new StatsCharacterModifier());
            statsModifiers.Add(new StatsRealmModifier());
            statsModifiers.Add(new StatsEssenceModifier());
            statsModifiers.Add(new StatsRaceModifier());
            stats.SetStatsModifier(statsModifiers);
        }
        Setup();
    }
    #endregion
    [ContextMenu("Test Stat Change")]
    public void Load()
    {
        piritRange = GetStatValue(StatType.SpiritRange);
    }
    public void StatChange()
    {
        OnValueChanged?.Invoke();
    }
}