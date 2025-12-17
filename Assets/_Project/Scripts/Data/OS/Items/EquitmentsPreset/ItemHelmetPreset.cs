#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewHelmetPreset", menuName = "RPG/Items/Equipment/Helmet Preset")]
public class ItemHelmetPreset : ItemEquipmentPreset
{
    [Header("Helmet")]
    public HelmetType helmetType;

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        equipmentType = EquipmentType.Helmet;

        ClearAllStats();

        // meta mặc định
        if (raceType == default) raceType = RaceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        if (qualityType == default) qualityType = QualityType.Mortal;

        itemName = $"{GetHelmetName(helmetType)} C{level}";

        switch (helmetType)
        {
            case HelmetType.NonVai:
                ApplyNonVai();
                break;

            case HelmetType.NonLua:
                ApplyNonLua();
                break;

            case HelmetType.NonBachNgan:
                ApplyNonBachNgan();
                break;
        }

#if UNITY_EDITOR
        string newName = $"Helmet_{itemName}_{qualityType}";
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

    private void ApplyNonVai()
    {
        level = 1;
        qualityType = QualityType.Mortal;
        physicalDefense = 0.20f;
    }

    private void ApplyNonLua()
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

    private void ApplyNonBachNgan()
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
                maxHealth = 0.20f;
                break;

            case QualityType.Earth:
                physicalDefense = 1.20f;
                maxHealth = 0.30f;
                break;

            case QualityType.Heaven:
                physicalDefense = 1.50f;
                maxHealth = 0.50f;
                maxMana = 0.50f;
                break;
        }
    }

    // ============================
    // HELPERS
    // ============================

    private static string GetHelmetName(HelmetType t)
    {
        return t switch
        {
            HelmetType.NonVai => "Nón vải",
            HelmetType.NonLua => "Nón lụa",
            HelmetType.NonBachNgan => "Nón bạch ngân",
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
