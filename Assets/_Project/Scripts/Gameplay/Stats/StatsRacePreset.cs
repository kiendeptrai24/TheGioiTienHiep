
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatsPreset", menuName = "RPG/Stats/Stats Race Preset")]
public class StatsRacePreset : ScriptableObject , IStatProvider
{
    [Header("Race type")]
    public RaceType raceType;

    [Header("Resources (multipliers or % as you like)")]
    public Stat health;   // Sinh lực
    public Stat mana;     // Linh lực
    public Stat spirit;   // Linh thức

    [Header("Offensive Stats")]
    public Stat physicalDamage; // Sát thương linh thể
    public Stat magicalDamage;  // Sát thương linh lực
    public Stat spiritDamage;   // Sát thương linh thức

    [Header("Defensive Stats")]
    public Stat physicalDefense; // Phòng ngự linh thể
    public Stat magicalDefense;  // Phòng ngự linh lực
    public Stat spiritDefense;   // Phòng ngự linh thức

    [Header("Speed / Range")]
    public Stat spiritRange;     // Phạm vi linh thức
    public Stat movementSpeed;   // Tddc

#if UNITY_EDITOR
    private void OnValidate()
    {
        string newName = $"Race_{raceType}";
        if (name != newName)
        {
            string path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();
            }
        }
    }
#endif

    [ContextMenu("Reset To Default (from table)")]
    public void ResetToDefault()
    {
        switch (raceType)
        {
            // Tộc Nhân
            // 10% 50% 20% | 10% 10% 10% | 10% 10% 10% | 0% 0%
            case RaceType.Human:
                ApplyRaceRow(
                    healthMul: 0.10f, manaMul: 0.50f, spiritMul: 0.20f,
                    physDmgMul: 0.10f, magDmgMul: 0.10f, spiritDmgMul: 0.10f,
                    physDefMul: 0.10f, magDefMul: 0.10f, spiritDefMul: 0.10f,
                    spiritRangeMul: 0.0f, moveSpeedMul: 0.0f
                );
                break;

            // Tộc Yêu
            // 50% 0% 0% | 0% 0% 0% | 50% 50% 50% | 0% 0%
            case RaceType.Beast:
                ApplyRaceRow(
                    healthMul: 0.50f, manaMul: 0.0f,  spiritMul: 0.0f,
                    physDmgMul: 0.0f,  magDmgMul: 0.0f, spiritDmgMul: 0.0f,
                    physDefMul: 0.50f, magDefMul: 0.50f, spiritDefMul: 0.50f,
                    spiritRangeMul: 0.0f, moveSpeedMul: 0.0f
                );
                break;

            // Tộc Thiên
            // 0% 0% 50% | 0% 0% 0% | 0% 0% 0% | 50% 0%
            case RaceType.Celestial:
                ApplyRaceRow(
                    healthMul: 0.0f,  manaMul: 0.0f, spiritMul: 0.50f,
                    physDmgMul: 0.0f, magDmgMul: 0.0f, spiritDmgMul: 0.0f,
                    physDefMul: 0.0f, magDefMul: 0.0f, spiritDefMul: 0.0f,
                    spiritRangeMul: 0.50f, moveSpeedMul: 0.0f
                );
                break;

            // Tộc Ma
            // 30% 10% 10% | 50% 50% 50% | 0% 0% 0% | 0% 0%
            case RaceType.Demon:
                ApplyRaceRow(
                    healthMul: 0.30f, manaMul: 0.10f, spiritMul: 0.10f,
                    physDmgMul: 0.50f, magDmgMul: 0.50f, spiritDmgMul: 0.50f,
                    physDefMul: 0.0f,  magDefMul: 0.0f,  spiritDefMul: 0.0f,
                    spiritRangeMul: 0.0f, moveSpeedMul: 0.0f
                );
                break;

            // Tộc Chung - Tộc
            case RaceType.General:
                ApplyRaceRow(
                    healthMul: 0.0f, manaMul: 0.0f, spiritMul: 0.0f,
                    physDmgMul: 0.0f, magDmgMul: 0.0f, spiritDmgMul: 0.0f,
                    physDefMul: 0.0f, magDefMul: 0.0f, spiritDefMul: 0.0f,
                    spiritRangeMul: 0.0f, moveSpeedMul: 0.0f
                );
                break;
        }
    }

    private void ApplyRaceRow(
        float healthMul, float manaMul, float spiritMul,
        float physDmgMul, float magDmgMul, float spiritDmgMul,
        float physDefMul, float magDefMul, float spiritDefMul,
        float spiritRangeMul, float moveSpeedMul)
    {
        // Resources
        health = new Stat(StatType.Health, healthMul);
        mana   = new Stat(StatType.Mana,   manaMul);
        spirit = new Stat(StatType.Spirit, spiritMul);

        // Offensive
        physicalDamage = new Stat(StatType.PhysicalDamage, physDmgMul);
        magicalDamage  = new Stat(StatType.MagicalDamage,  magDmgMul);
        spiritDamage   = new Stat(StatType.SpiritDamage,   spiritDmgMul);

        // Defensive
        physicalDefense = new Stat(StatType.PhysicalDefense, physDefMul);
        magicalDefense  = new Stat(StatType.MagicalDefense,  magDefMul);
        spiritDefense   = new Stat(StatType.SpiritDefense,   spiritDefMul);

        // Speed / Range
        spiritRange   = new Stat(StatType.SpiritRange,   spiritRangeMul);
        movementSpeed = new Stat(StatType.MovementSpeed, moveSpeedMul);
    }

    public void ApplyStats(Dictionary<StatType, Stat> stats)
    {
        stats.TryGetValue(StatType.Health, out Stat healthStat);
        stats.TryGetValue(StatType.Mana, out Stat manaStat);
        stats.TryGetValue(StatType.Spirit, out Stat spiritStat);
        stats.TryGetValue(StatType.PhysicalDamage, out Stat physicalDamageStat);
        stats.TryGetValue(StatType.MagicalDamage, out Stat magicalDamageStat);
        stats.TryGetValue(StatType.SpiritDamage, out Stat spiritDamageStat);
        stats.TryGetValue(StatType.PhysicalDefense, out Stat physicalDefenseStat);
        stats.TryGetValue(StatType.MagicalDefense, out Stat magicalDefenseStat);
        stats.TryGetValue(StatType.SpiritDefense, out Stat spiritDefenseStat);
        stats.TryGetValue(StatType.MovementSpeed, out Stat movementSpeedStat);
        stats.TryGetValue(StatType.SpiritRange, out Stat spiritRangeStat);

        healthStat.AddModifier(healthStat.GetValue() * health.GetValue());
        manaStat.AddModifier(manaStat.GetValue() * mana.GetValue());
        spiritStat.AddModifier(spiritStat.GetValue() * spirit.GetValue());
        physicalDamageStat.AddModifier(physicalDamageStat.GetValue() * physicalDamage.GetValue());
        magicalDamageStat.AddModifier(magicalDamageStat.GetValue() * magicalDamage.GetValue());
        spiritDamageStat.AddModifier(spiritDamageStat.GetValue() * spiritDamage.GetValue());
        physicalDefenseStat.AddModifier(physicalDefenseStat.GetValue() * physicalDefense.GetValue());
        magicalDefenseStat.AddModifier(magicalDefenseStat.GetValue() * magicalDefense.GetValue());
        spiritDefenseStat.AddModifier(spiritDefenseStat.GetValue() * spiritDefense.GetValue());
        movementSpeedStat.AddModifier(movementSpeedStat.GetValue() * movementSpeed.GetValue());
        spiritRangeStat.AddModifier(spiritRangeStat.GetValue() * spiritRange.GetValue());
    }
}