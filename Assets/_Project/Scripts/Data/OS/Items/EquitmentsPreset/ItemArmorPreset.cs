#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewArmorPreset", menuName = "RPG/Items/Equipment/Armor Preset")]
public class ItemArmorPreset : ItemEquipmentPreset
{
    [Header("Armor")]
    public ArmorType armorType;

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        // armor preset => luôn là Armor
        equipmentType = EquipmentType.Armor;

        // reset all stats về 0
        ClearAllStats();

        // default meta theo sheet
        if (raceType == default) raceType = RaceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        if (qualityType == default) qualityType = QuanlityType.Mortal;

        // set itemName theo armorType + Cấp + Phẩm
        itemName = $"{GetArmorName(armorType)} C{level}";

        // ---- APPLY THEO BẢNG ----
        // Bảng bạn đưa: armor chủ yếu tăng physicalDefense (Phòng ngự linh thể)
        // và riêng Giáp bạch ngân (C3) có thêm maxHealth/maxMana theo Phẩm ở bản Thiên.

        switch (armorType)
        {
            case ArmorType.GiaoVai:
                ApplyGiaoVai();
                break;

            case ArmorType.GiapLua:
                ApplyGiapLua();
                break;

            case ArmorType.GiapBachNgan:
                ApplyGiapBachNgan();
                break;
        }

#if UNITY_EDITOR
        // rename asset cho gọn
        string newName = $"Armor_{itemName}_{qualityType}";
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
    // APPLY PER ARMOR
    // ============================

    // 1C Giáo vải C1 Phàm: PhysicalDefense = 20%
    private void ApplyGiaoVai()
    {
        level = Mathf.Max(level, 1);
        qualityType = QuanlityType.Mortal;

        if (level == 1)
            physicalDefense = .2f;
    }

    // Giáp lụa C2: Phàm=30, Hoàng=40, Huyền=70 (all physicalDefense)
    private void ApplyGiapLua()
    {
        level = 2;

        physicalDefense = qualityType switch
        {
            QuanlityType.Mortal => .30f,
            QuanlityType.Yellow => .40f,
            QuanlityType.Mystic => .70f,
            _ => 0f
        };
    }

    // Giáp bạch ngân C3:
    // Phàm=60, Hoàng=70
    // Huyền=80 + AttackSpeed 20
    // Địa=120 + AttackSpeed 30
    // Thiên=150 + AttackSpeed 50 + MaxHealth 50 + MaxMana 50
    private void ApplyGiapBachNgan()
    {
        level = 3;

        switch (qualityType)
        {
            case QuanlityType.Mortal:
                physicalDefense = .60f;
                break;

            case QuanlityType.Yellow:
                physicalDefense = .70f;
                break;

            case QuanlityType.Mystic:
                physicalDefense = .80f;
                attackSpeed = .20f;
                break;

            case QuanlityType.Earth:
                physicalDefense = 1.20f;
                attackSpeed = .30f;
                break;

            case QuanlityType.Heaven:
                physicalDefense = 1.50f;
                maxHealth = .50f;
                maxMana = .50f;
                break;
        }
    }

    // ============================
    // HELPERS
    // ============================

    private static string GetArmorName(ArmorType t)
    {
        return t switch
        {
            ArmorType.GiaoVai => "Giáo vải",
            ArmorType.GiapLua => "Giáp lụa",
            ArmorType.GiapBachNgan => "Giáp bạch ngân",
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
