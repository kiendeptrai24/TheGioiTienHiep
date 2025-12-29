

using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : TGTHMonoBehaviour, ISaveable
{
    public event Action OnValueChanged;

    [Header("Preset base stats")]
    public StatsRaceData statsRaceData;
    public StatsCultivationPathData statsCultivationPathData;
    public StatsRealmData statsRealmData;
    private StatsModifier statsModifier = new StatsModifier();
    public Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();
    public int Health => GetStatValue(StatType.Health);
    public int Mana => GetStatValue(StatType.Mana);
    public int Spirit => GetStatValue(StatType.Spirit);
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
    #region Regen & Ally Regen
    public float HealthRegen => GetStatValue(StatType.HealthRegen);
    public float ManaRegen => GetStatValue(StatType.ManaRegen);
    public float SpiritRegen => GetStatValue(StatType.SpiritRegen);

    public float AllyHealthRegen => GetStatValue(StatType.AllyHealthRegen);
    public float AllyManaRegen => GetStatValue(StatType.AllyManaRegen);
    public float AllySpiritRegen => GetStatValue(StatType.AllySpiritRegen);
    #endregion
    #region Immunity & Ally Immunity
    public float DamageImmunity => GetStatValue(StatType.DamageImmunity);
    public float CCImmunity => GetStatValue(StatType.CCImmunity);
    public float FullImmunity => GetStatValue(StatType.FullImmunity);

    public float AllyDamageImmunity => GetStatValue(StatType.AllyDamageImmunity);
    public float AllyCCImmunity => GetStatValue(StatType.AllyCCImmunity);
    public float AllyFullImmunity => GetStatValue(StatType.AllyFullImmunity);
    #endregion
    #region Debuff
    public float HealingReduction => GetStatValue(StatType.HealingReduction);
    public float EnemyManaReduction => GetStatValue(StatType.EnemyManaReduction);
    public float EnemySpiritReduction => GetStatValue(StatType.EnemySpiritReduction);
    #endregion
    #region CC / Debuff
    public float Weaken => GetStatValue(StatType.Weaken);
    public float Paralyze => GetStatValue(StatType.Paralyze);
    public float Root => GetStatValue(StatType.Root);
    public float Stun => GetStatValue(StatType.Stun);
    public float Silence => GetStatValue(StatType.Silence);
    #endregion
    #region reaction and reduction of effects
    public float CCDurationReduction => GetStatValue(StatType.CCDurationReduction);
    public float CCResistance => GetStatValue(StatType.CCResistance);

    #endregion
    #region Speed
    public int MovementSpeed => GetStatValue(StatType.MovementSpeed);
    public int AttackSpeed => GetStatValue(StatType.AttackSpeed);
    public int CastSpeed => GetStatValue(StatType.CastSpeed);
    #endregion

    public int CombatPower => GetStatValue(StatType.CombatPower);
    protected override void Start()
    {
        base.Start();
    }
    private void InitStatsPreset()
    {
        ResetStatsModifiers();
    }

    [ContextMenu("Reset Stats Modifiers")]
    private void ResetStatsModifiers()
    {
        stats.Clear();
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            stats.Add(type, new Stat(type, 0f));
        }
    }

    public int GetStatValue(StatType type)
    {
        if (stats.TryGetValue(type, out Stat stat))
        {
            return Mathf.RoundToInt(stat.GetValue());
        }

        Debug.LogWarning($"Stat {type} không tồn tại trên {name}!");
        return 0;
    }
    public Stat GetStat(StatType type)
    {
        if (stats.TryGetValue(type, out Stat stat))
        {
            return stat;
        }
        return null;
    }
    [ContextMenu("Show Stats")]
    private void ShowStas()
    {
        string debugMsg = $"{name} Stats:\n";
        foreach (var stat in stats)
        {
            debugMsg += $"{stat.Key}: {stat.Value.GetValue()}\n";
        }
        Debug.Log(debugMsg);
    }

    public void LoadData(GameData _data)
    {
        ResetStatsModifiers();
        statsRaceData = _data.statsRaceData;
        statsCultivationPathData = _data.statsCultivationPathData;
        statsRealmData = _data.statsRealmData;
        statsModifier.AddStatsRaceData(stats, statsRaceData);
        statsModifier.AddStatsRealmData(stats, statsRealmData);
        statsModifier.AddStatsCultivationPathData(stats, statsCultivationPathData);
        ShowStas();
    }
    public void StatChange()
    {
        OnValueChanged.Invoke();
    }
    public void SaveGame(ref GameData _data)
    {
    }
}