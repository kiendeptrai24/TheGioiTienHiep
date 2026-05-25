#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewBeltPreset", menuName = "RPG/Items/Equipment/Belt Preset")]
public class ItemBeltPreset : ItemEquipmentPreset
{
    [Header("Belt")]
    public BeltType beltType;

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        equipmentType = EquipmentType.Belt;

        ClearAllStats();

        // meta mặc định
        if (raceType == default) raceType = RaceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        if (qualityType == default) qualityType = QuanlityType.Mortal;

        itemName = $"{GetBeltName(beltType)} C{level}";

        switch (beltType)
        {
            case BeltType.DaiLungVai:
                ApplyDaiLungVai();
                break;

            case BeltType.DaiLungLua:
                ApplyDaiLungLua();
                break;

            case BeltType.DaiLungBachNgan:
                ApplyDaiLungBachNgan();
                break;
        }

#if UNITY_EDITOR
        string newName = $"Belt_{itemName}_{qualityType}";
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

    // Đai lưng vải: C1 Phàm => physicalDefense = 20%
    private void ApplyDaiLungVai()
    {
        level = 1;
        qualityType = QuanlityType.Mortal;
        physicalDefense = 0.20f;
    }

    // Đai lưng lụa: C2 => Phàm 30, Hoàng 40, Huyền 70 (physicalDefense)
    private void ApplyDaiLungLua()
    {
        level = 2;

        physicalDefense = qualityType switch
        {
            QuanlityType.Mortal => 0.30f,
            QuanlityType.Yellow => 0.40f,
            QuanlityType.Mystic => 0.70f,
            _ => 0f
        };
    }

    // Đai lưng bạch ngân: C3
    // Phàm 60, Hoàng 70
    // Huyền 80 + reduceCritDamage 2
    // Địa 120 + reduceCritDamage 3
    // Thiên 150 + reduceCritDamage 5 + reduceArmorPen 5
    private void ApplyDaiLungBachNgan()
    {
        level = 3;

        switch (qualityType)
        {
            case QuanlityType.Mortal:
                physicalDefense = 0.60f;
                break;

            case QuanlityType.Yellow:
                physicalDefense = 0.70f;
                break;

            case QuanlityType.Mystic:
                physicalDefense = 0.80f;
                reduceCritDamage = 0.02f;
                break;

            case QuanlityType.Earth:
                physicalDefense = 1.20f;
                reduceCritDamage = 0.03f;
                break;

            case QuanlityType.Heaven:
                physicalDefense = 1.50f;
                reduceCritDamage = 0.05f;
                reduceArmorPen = 0.05f;
                break;
        }
    }

    // ============================
    // HELPERS
    // ============================

    private static string GetBeltName(BeltType t)
    {
        return t switch
        {
            BeltType.DaiLungVai => "Đai lưng vải",
            BeltType.DaiLungLua => "Đai lưng lụa",
            BeltType.DaiLungBachNgan => "Đai lưng bạch ngân",
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
