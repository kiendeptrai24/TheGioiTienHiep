

using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CharacterStats : TGTHNetworkBehaviour {
    [Header("Preset base stats")]
    public StatsPreset statsPreset;
    
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
        InitStatsFromPreset();
    }

    private void InitStatsFromPreset()
    {
         stats.Clear();

        if (statsPreset == null)
        {
            Debug.LogWarning($"{name} chưa gán StatsPreset!");
            return;
        }

        // Tạo Stat từ preset (base value)
        stats.Add(StatType.Health,           new Stat(StatType.Health,           statsPreset.health));
        stats.Add(StatType.Mana,             new Stat(StatType.Mana,             statsPreset.mana));
        stats.Add(StatType.Spirit,           new Stat(StatType.Spirit,           statsPreset.spirit));

        stats.Add(StatType.PhysicalDamage,   new Stat(StatType.PhysicalDamage,   statsPreset.physicalDamage));
        stats.Add(StatType.MagicalDamage,    new Stat(StatType.MagicalDamage,    statsPreset.magicalDamage));
        stats.Add(StatType.SpiritDamage,     new Stat(StatType.SpiritDamage,     statsPreset.spiritDamage));
        stats.Add(StatType.CritChance,       new Stat(StatType.CritChance,       statsPreset.critChance));
        stats.Add(StatType.CritPower,        new Stat(StatType.CritPower,        statsPreset.critPower));

        stats.Add(StatType.PhysicalDefense,  new Stat(StatType.PhysicalDefense,  statsPreset.physicalDefense));
        stats.Add(StatType.MagicalDefense,   new Stat(StatType.MagicalDefense,   statsPreset.magicalDefense));
        stats.Add(StatType.SpiritDefense,    new Stat(StatType.SpiritDefense,    statsPreset.spiritDefense));
        stats.Add(StatType.Evasion,          new Stat(StatType.Evasion,          statsPreset.evasion));
        stats.Add(StatType.SpiritPenetration,new Stat(StatType.SpiritPenetration,statsPreset.spiritPenetration));
        stats.Add(StatType.MindPenetration,  new Stat(StatType.MindPenetration,  statsPreset.mindPenetration));

        stats.Add(StatType.MovementSpeed,    new Stat(StatType.MovementSpeed,    statsPreset.movementSpeed));
        stats.Add(StatType.AttackSpeed,      new Stat(StatType.AttackSpeed,      statsPreset.attackSpeed));
        stats.Add(StatType.CastSpeed,        new Stat(StatType.CastSpeed,        statsPreset.castSpeed));

        stats.Add(StatType.Potential,        new Stat(StatType.Potential,        statsPreset.potential));
        stats.Add(StatType.SkillPoints,      new Stat(StatType.SkillPoints,      statsPreset.skillPoints));
        stats.Add(StatType.CombatPower,      new Stat(StatType.CombatPower,      statsPreset.combatPower));

        // Nếu muốn đảm bảo mọi StatType đều tồn tại (kể cả chưa set trong preset)
        foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
        {
            if (!stats.ContainsKey(type))
            {
                stats.Add(type, new Stat(type, 0f));
            }
        }
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
    protected override void Start() {
        string debugMsg = $"{name} Stats:\n";
        foreach(var stat in stats)
        {
            debugMsg += $"{stat.Key}: {stat.Value.GetValue()}\n";
        }
        Debug.Log(debugMsg);
    }
}