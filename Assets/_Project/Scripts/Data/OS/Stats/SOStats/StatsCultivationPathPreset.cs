
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewStatsPreset", menuName = "RPG/Stats/Stats Cultivation Path Preset")]
public class StatsCultivationPathPreset : ScriptableObject
{
    [Header("Main cultivation type")]
    public EssenceType essenceType;

    [Header("Counter cultivation type")]
    public EssenceType counterEssenceType;

    // 0.2 = giảm 20% phòng ngự của essence bị khắc chế
    [Range(0f, 1f)]
    public float counterPercentage;

    [Header("Resources (per point)")]
    public float health;
    public float mana;
    public float spirit;

    [Header("Offensive Stats (per point)")]
    public float physicalDamage;
    public float magicalDamage;
    public float spiritDamage;

    [Header("Defensive Stats (per point)")]
    public float physicalDefense;
    public float magicalDefense;
    public float spiritDefense;

    [Header("Speed / Range (per point)")]
    public float movementSpeed;
    public float spiritRange;

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
        health = sinhLuc;
        mana   = linhLuc;
        spirit = linhThuc;

        // Offensive
        physicalDamage = satThuongLinhThe;
        magicalDamage  = satThuongLinhLuc;
        spiritDamage   = satThuongLinhThuc;

        // Defensive
        physicalDefense = phongNguLinhThe;
        magicalDefense  = phongNguLinhLuc;
        spiritDefense   = phongNguLinhThuc;

        // Speed / Range
        movementSpeed = tocDoDiChuyen;
        spiritRange   = phamViLinhThuc;

        // Counter info
        counterEssenceType = counterType;
        counterPercentage  = counterPercent;
    }
    public StatsCultivationPathData GetStats()
    {
        StatsCultivationPathData data = new StatsCultivationPathData();
        data.essenceType = essenceType;
        data.counterEssenceType = counterEssenceType;
        data.counterPercentage = counterPercentage;
        data.health = Mathf.RoundToInt(health);
        data.mana = Mathf.RoundToInt(mana);
        data.spirit = Mathf.RoundToInt(spirit);
        data.physicalDamage = Mathf.RoundToInt(physicalDamage);
        data.magicalDamage = Mathf.RoundToInt(magicalDamage);
        data.spiritDamage = Mathf.RoundToInt(spiritDamage);
        data.physicalDefense = Mathf.RoundToInt(physicalDefense);
        data.magicalDefense = Mathf.RoundToInt(magicalDefense);
        data.spiritDefense = Mathf.RoundToInt(spiritDefense);
        data.movementSpeed = Mathf.RoundToInt(movementSpeed);
        data.spiritRange = Mathf.RoundToInt(spiritRange);
        return data;
    }
}