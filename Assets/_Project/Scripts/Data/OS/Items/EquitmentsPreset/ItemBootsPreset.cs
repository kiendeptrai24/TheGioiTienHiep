#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewBootsPreset", menuName = "RPG/Items/Equipment/Boots Preset")]
public class ItemBootsPreset : ItemEquipmentPreset
{
    [Header("Boots")]
    public BootsType bootsType;

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        equipmentType = EquipmentType.Boots;

        ClearAllStats();

        // default meta
        if (raceType == default) raceType = RaceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        if (qualityType == default) qualityType = QuanlityType.Mortal;

        itemName = $"{GetBootsName(bootsType)} C{level}";

        switch (bootsType)
        {
            case BootsType.GiayVai:
                ApplyGiayVai();
                break;

            case BootsType.GiayLua:
                ApplyGiayLua();
                break;

            case BootsType.GiayBachNgan:
                ApplyGiayBachNgan();
                break;
        }

#if UNITY_EDITOR
        string newName = $"Boots_{itemName}_{qualityType}";
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

    // Giày vải: C1 Phàm => moveSpeed 4%
    private void ApplyGiayVai()
    {
        level = 1;
        qualityType = QuanlityType.Mortal;
        moveSpeed = 0.04f;
    }

    // Giày lụa: C2 => moveSpeed: Phàm 6, Hoàng 8, Huyền 14
    private void ApplyGiayLua()
    {
        level = 2;

        moveSpeed = qualityType switch
        {
            QuanlityType.Mortal => 0.06f,
            QuanlityType.Yellow => 0.08f,
            QuanlityType.Mystic => 0.14f,
            _ => 0f
        };
    }

    // Giày bạch ngân: C3
    // Phàm 12% MS, Hoàng 14% MS
    // Huyền: bonusHealth 20% + MS 16%
    // Địa:  bonusHealth 30% + MS 24%
    // Thiên: bonusHealth 50% + bonusMana 50% + MS 30% + attackSpeed 50% (theo hàng bạn paste)
    private void ApplyGiayBachNgan()
    {
        level = 3;

        switch (qualityType)
        {
            case QuanlityType.Mortal:
                moveSpeed = 0.12f;
                break;

            case QuanlityType.Yellow:
                moveSpeed = 0.14f;
                break;

            case QuanlityType.Mystic:
                physicalDefense = 0.20f;
                moveSpeed = 0.16f;
                break;

            case QuanlityType.Earth:
                physicalDefense = 0.30f;
                moveSpeed = 0.24f;
                break;

            case QuanlityType.Heaven:
                // hàng Thiên của bạn có: attackSpeed 50% + bonusHealth 50% + moveSpeed 30% + bonusMana 50%
                attackSpeed = 0.50f;
                physicalDefense = 0.50f;
                magicalDefense = 0.50f;
                moveSpeed = 0.30f;
                break;
        }
    }

    // ============================
    // HELPERS
    // ============================

    private static string GetBootsName(BootsType t)
    {
        return t switch
        {
            BootsType.GiayVai => "Giày vải",
            BootsType.GiayLua => "Giày lụa",
            BootsType.GiayBachNgan => "Giày bạch ngân",
            _ => t.ToString()
        };
    }

    private void ClearAllStats()
    {
        // Damage
        physicalDamage = magicalDamage = spiritDamage = 0;
        criticalDamage = criticalRate = 0;
        trueDamage = armorPenetration = lifeSteal = attackSpeed = 0;

        // Defense / Resource bonus
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
