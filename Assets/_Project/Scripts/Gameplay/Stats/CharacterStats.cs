using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : TGTHNetworkBehaviour
{
    [Header("Preset base stats")]
    public StatsRealmPreset statsRealmPreset;
    public StatsRacePreset statsRacePreset;
    public StatsCultivationPathPreset StatsCultivationPathPreset;

    // Dictionary runtime chứa toàn bộ Stat
    private Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

    // Ví dụ expose property cho dễ gọi
    public int Health => GetStatValue(StatType.Health);
    public int Mana => GetStatValue(StatType.Mana);
    public int Spirit => GetStatValue(StatType.Spirit);

    public int PhysicalDamage => GetStatValue(StatType.PhysicalDamage);
    public int MagicalDamage => GetStatValue(StatType.MagicalDamage);
    public int SpiritDamage => GetStatValue(StatType.SpiritDamage);

    public int PhysicalDefense => GetStatValue(StatType.PhysicalDefense);
    public int MagicalDefense => GetStatValue(StatType.MagicalDefense);
    public int SpiritDefense => GetStatValue(StatType.SpiritDefense);

    public int MovementSpeed => GetStatValue(StatType.MovementSpeed);
    public int AttackSpeed => GetStatValue(StatType.AttackSpeed);
    public int CastSpeed => GetStatValue(StatType.CastSpeed);

    public int CombatPower => GetStatValue(StatType.CombatPower);
    
    protected override void Awake()
    {
        InitStatsPreset();
    }

    private void InitStatsPreset()
    {
        ResetStatsModifiers();
        AddStatsFromPreset();
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
    
    [ContextMenu("Add Stats From Preset")]
    private void AddStatsFromPreset()
    {
        statsRealmPreset.ApplyStats(stats);
        StatsCultivationPathPreset.ApplyStats(stats);
        statsRacePreset.ApplyStats(stats);
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

    protected override void Start()
    {
        string debugMsg = $"{name} Stats:\n";
        foreach (var stat in stats)
        {
            debugMsg += $"{stat.Key}: {stat.Value.GetValue()}\n";
        }
        Debug.Log(debugMsg);

    }
}