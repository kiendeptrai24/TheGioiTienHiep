
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewStatsPreset", menuName = "RPG/Stats/Stats Cultivation Path Preset")]
public class StatsCultivationPathPreset : ScriptableObject , IStatProvider
{
    [Header("Main cultivation type")]
    public EssenceType essenceType;

    [Header("Counter cultivation type")]
    public EssenceType counterEssenceType;

    // 0.2 = giảm 20% phòng ngự của essence bị khắc chế
    [Range(0f, 1f)]
    public float counterPercentage;

    [Header("Resources (per point)")]
    public Stat health;
    public Stat mana;
    public Stat spirit;

    [Header("Offensive Stats (per point)")]
    public Stat physicalDamage;
    public Stat magicalDamage;
    public Stat spiritDamage;

    [Header("Defensive Stats (per point)")]
    public Stat physicalDefense;
    public Stat magicalDefense;
    public Stat spiritDefense;

    [Header("Speed / Range (per point)")]
    public Stat movementSpeed;
    public Stat spiritRange;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Auto rename asset cho dễ nhìn
        string newName = $"CultivationPath_{essenceType}";
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

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        switch (essenceType)
        {
            case EssenceType.Physical:
                ApplyRow(
                    sinhLuc: 10f, linhLuc: 5f,  linhThuc: 5f,
                    satThuongLinhThe: 1f, satThuongLinhLuc: 0f, satThuongLinhThuc: 0f,
                    phongNguLinhThe: 1f, phongNguLinhLuc: 0f, phongNguLinhThuc: 0f,
                    phamViLinhThuc: 1f, tocDoDiChuyen: 1f,
                    counterType: EssenceType.Spirit,
                    counterPercent: 0.20f
                );
                break;
            case EssenceType.Magical:
                ApplyRow(
                    sinhLuc: 5f, linhLuc: 10f, linhThuc: 5f,
                    satThuongLinhThe: 0f, satThuongLinhLuc: 1f, satThuongLinhThuc: 0f,
                    phongNguLinhThe: 0f, phongNguLinhLuc: 1f, phongNguLinhThuc: 0f,
                    phamViLinhThuc: 1f, tocDoDiChuyen: 1f,
                    counterType: EssenceType.Physical,
                    counterPercent: 0.20f
                );
                break;

            case EssenceType.Spirit:
                ApplyRow(
                    sinhLuc: 5f, linhLuc: 5f, linhThuc: 10f,
                    satThuongLinhThe: 0f, satThuongLinhLuc: 0f, satThuongLinhThuc: 1f,
                    phongNguLinhThe: 0f, phongNguLinhLuc: 0f, phongNguLinhThuc: 1f,
                    phamViLinhThuc: 1f, tocDoDiChuyen: 1f,
                    counterType: EssenceType.Magical,
                    counterPercent: 0.20f
                );
                break;

            case EssenceType.General:
                ApplyRow(
                    sinhLuc: 0f, linhLuc: 0f, linhThuc: 0f,
                    satThuongLinhThe: 0f, satThuongLinhLuc: 0f, satThuongLinhThuc: 0f,
                    phongNguLinhThe: 0f, phongNguLinhLuc: 0f, phongNguLinhThuc: 0f,
                    phamViLinhThuc: 0f, tocDoDiChuyen: 0f,
                    counterType: EssenceType.General,
                    counterPercent: 0f
                );
                break;
        }
    }

    private void ApplyRow(
        float sinhLuc, float linhLuc, float linhThuc,
        float satThuongLinhThe, float satThuongLinhLuc, float satThuongLinhThuc,
        float phongNguLinhThe, float phongNguLinhLuc, float phongNguLinhThuc,
        float phamViLinhThuc, float tocDoDiChuyen,
        EssenceType counterType, float counterPercent)
    {
        // Resources
        health = new Stat(StatType.Health, sinhLuc);
        mana   = new Stat(StatType.Mana,   linhLuc);
        spirit = new Stat(StatType.Spirit, linhThuc);

        // Offensive
        physicalDamage = new Stat(StatType.PhysicalDamage, satThuongLinhThe);
        magicalDamage  = new Stat(StatType.MagicalDamage,  satThuongLinhLuc);
        spiritDamage   = new Stat(StatType.SpiritDamage,   satThuongLinhThuc);

        // Defensive
        physicalDefense = new Stat(StatType.PhysicalDefense, phongNguLinhThe);
        magicalDefense  = new Stat(StatType.MagicalDefense,  phongNguLinhLuc);
        spiritDefense   = new Stat(StatType.SpiritDefense,   phongNguLinhThuc);

        // Speed / Range
        movementSpeed = new Stat(StatType.MovementSpeed, tocDoDiChuyen);
        spiritRange   = new Stat(StatType.SpiritRange,   phamViLinhThuc);

        // Counter info
        counterEssenceType = counterType;
        counterPercentage  = counterPercent;
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

        // Resources
        healthStat.AddModifier(health.GetValue());
        manaStat.AddModifier(mana.GetValue());
        spiritStat.AddModifier(spirit.GetValue());

        // Offensive
        physicalDamageStat.AddModifier(physicalDamage.GetValue());
        magicalDamageStat.AddModifier(magicalDamage.GetValue());
        spiritDamageStat.AddModifier(spiritDamage.GetValue());

        // Defensive
        physicalDefenseStat.AddModifier(physicalDefense.GetValue());
        magicalDefenseStat.AddModifier(magicalDefense.GetValue());
        spiritDefenseStat.AddModifier(spiritDefense.GetValue());

        // Speed / Range
        movementSpeedStat.AddModifier(movementSpeed.GetValue());
        spiritRangeStat.AddModifier(spiritRange.GetValue());
    }
    public StatsCultivationPathData GetStats()
    {
        StatsCultivationPathData data = new StatsCultivationPathData();
        data.essenceType = essenceType;
        data.counterEssenceType = counterEssenceType;
        data.counterPercentage = counterPercentage;
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