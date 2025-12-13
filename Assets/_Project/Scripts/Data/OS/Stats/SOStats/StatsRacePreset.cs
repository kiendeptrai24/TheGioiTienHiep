
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStatsPreset", menuName = "RPG/Stats/Stats Race Preset")]
public class StatsRacePreset : ScriptableObject , IStatProvider
{
    [Header("Race type")]
    public RaceType raceType;

    [Header("Resources (multipliers or % as you like)")]
    public Stat health;
    public Stat mana;
    public Stat spirit;

    [Header("Offensive Stats")]
    public Stat physicalDamage;
    public Stat magicalDamage;
    public Stat spiritDamage;

    [Header("Defensive Stats")]
    public Stat physicalDefense;
    public Stat magicalDefense;
    public Stat spiritDefense;

    [Header("Speed / Range")]
    public Stat spiritRange;
    public Stat movementSpeed;

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
            case RaceType.Human:
                ApplyRaceRow(
                    healthMul: 0.10f, manaMul: 0.50f, spiritMul: 0.20f,
                    physDmgMul: 0.10f, magDmgMul: 0.10f, spiritDmgMul: 0.10f,
                    physDefMul: 0.10f, magDefMul: 0.10f, spiritDefMul: 0.10f,
                    spiritRangeMul: 0.0f, moveSpeedMul: 0.0f
                );
                break;

            case RaceType.Beast:
                ApplyRaceRow(
                    healthMul: 0.50f, manaMul: 0.0f,  spiritMul: 0.0f,
                    physDmgMul: 0.0f,  magDmgMul: 0.0f, spiritDmgMul: 0.0f,
                    physDefMul: 0.50f, magDefMul: 0.50f, spiritDefMul: 0.50f,
                    spiritRangeMul: 0.0f, moveSpeedMul: 0.0f
                );
                break;

            case RaceType.Celestial:
                ApplyRaceRow(
                    healthMul: 0.0f,  manaMul: 0.0f, spiritMul: 0.50f,
                    physDmgMul: 0.0f, magDmgMul: 0.0f, spiritDmgMul: 0.0f,
                    physDefMul: 0.0f, magDefMul: 0.0f, spiritDefMul: 0.0f,
                    spiritRangeMul: 0.50f, moveSpeedMul: 0.0f
                );
                break;

            case RaceType.Demon:
                ApplyRaceRow(
                    healthMul: 0.30f, manaMul: 0.10f, spiritMul: 0.10f,
                    physDmgMul: 0.50f, magDmgMul: 0.50f, spiritDmgMul: 0.50f,
                    physDefMul: 0.0f,  magDefMul: 0.0f,  spiritDefMul: 0.0f,
                    spiritRangeMul: 0.0f, moveSpeedMul: 0.0f
                );
                break;

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
    public StatsRaceData GetStats()
    {
        StatsRaceData data = new StatsRaceData();
        data.raceType = raceType;
        data.health = Mathf.RoundToInt(health.GetValue());
        data.mana = Mathf.RoundToInt(mana.GetValue());
        data.spirit = Mathf.RoundToInt(spirit.GetValue());
        data.physicalDamage = Mathf.RoundToInt(physicalDamage.GetValue());
        data.magicalDamage = Mathf.RoundToInt(magicalDamage.GetValue());
        data.spiritDamage = Mathf.RoundToInt(spiritDamage.GetValue());
        data.physicalDefense = Mathf.RoundToInt(physicalDefense.GetValue());
        data.magicalDefense = Mathf.RoundToInt(magicalDefense.GetValue());
        data.spiritDefense = Mathf.RoundToInt(spiritDefense.GetValue());
        data.movementSpeed = Mathf.RoundToInt(movementSpeed.GetValue());
        data.spiritRange = Mathf.RoundToInt(spiritRange.GetValue());
        return data;
    }

}