#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewPantsPreset", menuName = "RPG/Items/Equipment/Pants Preset")]
public class ItemPantsPreset : ItemEquipmentPreset
{
    [Header("Pants")]
    public PantsType pantsType;

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        equipmentType = EquipmentType.Pants;

        ClearAllStats();

        // meta mặc định
        if (raceType == default) raceType = RaceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        if (qualityType == default) qualityType = QualityType.Mortal;

        itemName = $"{GetPantsName(pantsType)} C{level}";

        switch (pantsType)
        {
            case PantsType.QuanVai:
                ApplyQuanVai();
                break;

            case PantsType.QuanLua:
                ApplyQuanLua();
                break;

            case PantsType.QuanBachNgan:
                ApplyQuanBachNgan();
                break;
        }

#if UNITY_EDITOR
        string newName = $"Pants_{itemName}_{qualityType}";
        if (name != newName)
        {
            string path = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }

    // ============================
    // APPLY (0–1 SCALE)
    // ============================

    // Quần vải: C1 – Phàm – 20%
    private void ApplyQuanVai()
    {
        level = 1;
        qualityType = QualityType.Mortal;
        physicalDefense = 0.20f;
    }

    // Quần lụa: C2
    // Phàm 30% | Hoàng 40% | Huyền 70%
    private void ApplyQuanLua()
    {
        level = 2;

        physicalDefense = qualityType switch
        {
            QualityType.Mortal => 0.30f,
            QualityType.Yellow => 0.40f,
            QualityType.Mystic => 0.70f,
            _ => 0f
        };
    }

    // Quần bạch ngân: C3
    // Phàm 60%
    // Hoàng 70%
    // Huyền 80% + giảm crit 2%
    // Địa 120% + giảm crit 3%
    // Thiên 150% + giảm crit 5% + giảm xuyên giáp 5%
    private void ApplyQuanBachNgan()
    {
        level = 3;

        switch (qualityType)
        {
            case QualityType.Mortal:
                physicalDefense = 0.60f;
                break;

            case QualityType.Yellow:
                physicalDefense = 0.70f;
                break;

            case QualityType.Mystic:
                physicalDefense = 0.80f;
                reduceCritDamage = 0.02f;
                break;

            case QualityType.Earth:
                physicalDefense = 1.20f;
                reduceCritDamage = 0.03f;
                break;

            case QualityType.Heaven:
                physicalDefense = 1.50f;
                reduceCritDamage = 0.05f;
                reduceArmorPen = 0.05f;
                break;
        }
    }

    // ============================
    // HELPERS
    // ============================

    private static string GetPantsName(PantsType t)
    {
        return t switch
        {
            PantsType.QuanVai => "Quần vải",
            PantsType.QuanLua => "Quần lụa",
            PantsType.QuanBachNgan => "Quần bạch ngân",
            _ => t.ToString()
        };
    }

    private void ClearAllStats()
    {
        // Damage
        physicalDamage = magicalDamage = spiritDamage = 0;
        criticalDamage = criticalRate = 0;
        trueDamage = armorPenetration = lifeSteal = attackSpeed = 0;

        // Defense
        maxHealth = maxMana = maxSpirit = 0;
        physicalDefense = magicalDefense = spiritDefense = 0;
        healthRegen = manaRegen = spiritRegen = 0;
        allyHealthRegen = allyManaRegen = allySpiritRegen = 0;
        reduceCritDamage = reduceArmorPen = reduceTrueDamage = 0;
        reflectDamage = moveSpeed = 0;

        // Effects
        immuneAllyDamage = immuneAllyEffects = immuneAllFromAllies = 0;
        cleanseAllyEffects = 0;
        grievousWound = reduceEnemyMana = reduceEnemySpirit = 0;
        weakenTarget = paralyzeChance = rootChance = stunChance = silenceChance = 0;
        immuneDamage = immuneEffects = immuneAll = 0;
        reduceEffectDuration = effectResistance = 0;
    }
}
