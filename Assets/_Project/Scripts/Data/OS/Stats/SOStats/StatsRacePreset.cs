
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStatsPreset", menuName = "RPG/Stats/Stats Race Preset")]
public class StatsRacePreset : ScriptableObject
{
    [Header("Race type")]
    public RaceType raceType;

    [Header("Resources (multipliers or % as you like)")]
    public float health;
    public float mana;
    public float spirit;

    [Header("Offensive Stats")]
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;

    [Header("Defensive Stats")]
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;

    [Header("Speed / Range")]
    public float spiritRange;
    public float movementSpeed;

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
                    healthMul: 0.50f, manaMul: 0.0f, spiritMul: 0.0f,
                    physDmgMul: 0.0f, magDmgMul: 0.0f, spiritDmgMul: 0.0f,
                    physDefMul: 0.50f, magDefMul: 0.50f, spiritDefMul: 0.50f,
                    spiritRangeMul: 0.0f, moveSpeedMul: 0.0f
                );
                break;

            case RaceType.Celestial:
                ApplyRaceRow(
                    healthMul: 0.0f, manaMul: 0.0f, spiritMul: 0.50f,
                    physDmgMul: 0.0f, magDmgMul: 0.0f, spiritDmgMul: 0.0f,
                    physDefMul: 0.0f, magDefMul: 0.0f, spiritDefMul: 0.0f,
                    spiritRangeMul: 0.50f, moveSpeedMul: 0.0f
                );
                break;

            case RaceType.Demon:
                ApplyRaceRow(
                    healthMul: 0.30f, manaMul: 0.10f, spiritMul: 0.10f,
                    physDmgMul: 0.50f, magDmgMul: 0.50f, spiritDmgMul: 0.50f,
                    physDefMul: 0.0f, magDefMul: 0.0f, spiritDefMul: 0.0f,
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
        health = healthMul;
        mana = manaMul;
        spirit = spiritMul;

        // Offensive
        physicalDamage = physDmgMul;
        magicalDamage = magDmgMul;
        spiritDamage = spiritDmgMul;

        // Defensive
        physicalDefense = physDefMul;
        magicalDefense = magDefMul;
        spiritDefense = spiritDefMul;

        // Speed / Range
        spiritRange = spiritRangeMul;
        movementSpeed = moveSpeedMul;
    }

    public RaceData GetStats()
    {
        RaceData data = new RaceData();
        data.raceType = raceType;
        data.healthPoint = health;
        data.manaPoint = mana;
        data.spiritPoint = spirit;
        data.physicalDamage = physicalDamage;
        data.magicalDamage = magicalDamage;
        data.spiritDamage = spiritDamage;
        data.physicalDefense = physicalDefense;
        data.magicalDefense = magicalDefense;
        data.spiritDefense = spiritDefense;
        data.movementSpeedPoint = movementSpeed;
        data.spiritRangePoint = spiritRange;
        return data;
    }

}