

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CharacterStats : TGTHNetworkBehaviour
{
    [Header("Preset base stats")]
    public StatsRealmPreset statsRealmPreset;
    public StatsRacePreset statsRacePreset;
    public StatsCultivationPathPreset StatsCultivationPathPreset;

    // Dictionary runtime chứa toàn bộ Stat
    private Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

    // Ví dụ expose property cho dễ gọi
    public float Health => GetStatValue(StatType.Health);
    public float Mana => GetStatValue(StatType.Mana);
    public float Spirit => GetStatValue(StatType.Spirit);

    public float PhysicalDamage => GetStatValue(StatType.PhysicalDamage);
    public float MagicalDamage => GetStatValue(StatType.MagicalDamage);
    public float SpiritDamage => GetStatValue(StatType.SpiritDamage);

    public float PhysicalDefense => GetStatValue(StatType.PhysicalDefense);
    public float MagicalDefense => GetStatValue(StatType.MagicalDefense);
    public float SpiritDefense => GetStatValue(StatType.SpiritDefense);

    public float MovementSpeed => GetStatValue(StatType.MovementSpeed);
    public float AttackSpeed => GetStatValue(StatType.AttackSpeed);
    public float CastSpeed => GetStatValue(StatType.CastSpeed);

    public float CombatPower => GetStatValue(StatType.CombatPower);
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
        statsRacePreset.ApplyStats(stats);
        StatsCultivationPathPreset.ApplyStats(stats);
    }
    public float GetStatValue(StatType type)
    {
        if (stats.TryGetValue(type, out Stat stat))
        {
            return stat.GetValue();
        }

        Debug.LogWarning($"Stat {type} không tồn tại trên {name}!");
        return 0f;
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