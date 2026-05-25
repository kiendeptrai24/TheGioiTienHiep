#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlovesPreset", menuName = "RPG/Items/Equipment/Gloves Preset")]
public class ItemGlovesPreset : ItemEquipmentPreset
{
    [Header("Gloves")]
    public GlovesType glovesType;

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        equipmentType = EquipmentType.Gloves; // ⚠️ Nếu bạn có EquipmentType.Gloves thì đổi lại. Nếu không có, tạm dùng Bracelet/None tùy bạn.
        // Gợi ý: thêm EquipmentType.Gloves vào enum EquipmentType để đúng nghĩa.

        ClearAllStats();

        // default meta
        if (raceType == default) raceType = RaceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        if (qualityType == default) qualityType = QuanlityType.Mortal;

        itemName = $"{GetGlovesName(glovesType)} C{level}";

        switch (glovesType)
        {
            case GlovesType.BaoTayVai:
                ApplyBaoTayVai();
                break;

            case GlovesType.BaoTayLua:
                ApplyBaoTayLua();
                break;

            case GlovesType.BaoTayBachNgan:
                ApplyBaoTayBachNgan();
                break;
        }

#if UNITY_EDITOR
        string newName = $"Gloves_{itemName}_{qualityType}";
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

    // Bao tay vải: C1 Phàm => physicalDamage 20%
    private void ApplyBaoTayVai()
    {
        level = 1;
        qualityType = QuanlityType.Mortal;
        physicalDamage = 0.20f;
    }

    // Bao tay lụa: C2 => physicalDamage: Phàm 30, Hoàng 40, Huyền 70
    private void ApplyBaoTayLua()
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

    // Bao tay bạch ngân: C3
    // Phàm 60% dmg, Hoàng 70% dmg
    // Huyền: 80% dmg + critRate 4%
    // Địa : 120% dmg + critRate 6%
    // Thiên: 150% dmg + critDamage 50% + critRate 10%
    private void ApplyBaoTayBachNgan()
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

    private static string GetGlovesName(GlovesType t)
    {
        return t switch
        {
            GlovesType.BaoTayVai => "Bao tay vải",
            GlovesType.BaoTayLua => "Bao tay lụa",
            GlovesType.BaoTayBachNgan => "Bao tay bạch ngân",
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
