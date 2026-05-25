#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewNecklacePreset", menuName = "RPG/Items/Equipment/Necklace Preset")]
public class ItemNecklacePreset : ItemEquipmentPreset
{
    [Header("Necklace")]
    public NecklaceType necklaceType;

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        equipmentType = EquipmentType.Necklace;

        ClearAllStats();

        // default meta
        if (raceType == default) raceType = RaceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        if (qualityType == default) qualityType = QuanlityType.Mortal;

        itemName = $"{GetNecklaceName(necklaceType)} C{level}";

        switch (necklaceType)
        {
            case NecklaceType.DayChuyenCo:
                ApplyDayChuyenCo();
                break;

            case NecklaceType.DayChuyenThuyTinh:
                ApplyDayChuyenThuyTinh();
                break;

            case NecklaceType.DayChuyenBachNgan:
                ApplyDayChuyenBachNgan();
                break;
        }

#if UNITY_EDITOR
        string newName = $"Necklace_{itemName}_{qualityType}";
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

    // Dây chuyền cỏ: C1 Phàm => physicalDamage 20%
    private void ApplyDayChuyenCo()
    {
        level = 1;
        qualityType = QuanlityType.Mortal;
        physicalDamage = 0.20f;
    }

    // Dây chuyền thủy tinh: C2 => physicalDamage: Phàm 30, Hoàng 40, Huyền 70
    private void ApplyDayChuyenThuyTinh()
    {
        level = 2;

        physicalDamage = qualityType switch
        {
            QuanlityType.Mortal => 0.30f,
            QuanlityType.Yellow => 0.40f,
            QuanlityType.Mystic => 0.70f,
            _ => 0f
        };
    }

    // Dây chuyền bạch ngân: C3
    // Phàm 60% dmg, Hoàng 70% dmg
    // Huyền: 80% dmg + critRate 4%
    // Địa : 120% dmg + critRate 6%
    // Thiên: 150% dmg + critDamage 50% + critRate 10%
    private void ApplyDayChuyenBachNgan()
    {
        level = 3;

        switch (qualityType)
        {
            case QuanlityType.Mortal:
                physicalDamage = 0.60f;
                break;

            case QuanlityType.Yellow:
                physicalDamage = 0.70f;
                break;

            case QuanlityType.Mystic:
                physicalDamage = 0.80f;
                criticalRate = 0.04f;
                break;

            case QuanlityType.Earth:
                physicalDamage = 1.20f;
                criticalRate = 0.06f;
                break;

            case QuanlityType.Heaven:
                physicalDamage = 1.50f;
                criticalDamage = 0.50f;
                criticalRate = 0.10f;
                break;
        }
    }

    // ============================
    // HELPERS
    // ============================

    private static string GetNecklaceName(NecklaceType t)
    {
        return t switch
        {
            NecklaceType.DayChuyenCo => "Dây chuyền cỏ",
            NecklaceType.DayChuyenThuyTinh => "Dây chuyền thủy tinh",
            NecklaceType.DayChuyenBachNgan => "Dây chuyền bạch ngân",
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
