


using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponPreset", menuName = "RPG/Items/Equipment/Weapon Preset")]
public class ItemWeaponPreset : ItemEquipmentPreset
{
    public WeaponType weaponType;
    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        equipmentType = EquipmentType.Weapon;
        ClearStats();
        ApplyCommonMetaDefaults();

        if (!TryApplyFromTable(itemName, level, qualityType))
        {
            Debug.LogWarning($"[ItemEquipmentPreset] Không tìm thấy data auto cho: name='{itemName}', level={level}, quality={qualityType}", this);
        }

#if UNITY_EDITOR
        string newName = $"Equipment_{itemName}_C{level}_{qualityType}";
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

    private void ApplyCommonMetaDefaults()
    {
        // theo sheet của bạn đa số: Chung/Chung
        if (raceType == default) raceType = RaceType.General;
        if (elementType == default) elementType = ElementType.Neutral;
        // qualityType và level thường bạn set sẵn trên asset
    }

    private void ClearStats()
    {
        physicalDamage = magicalDamage = spiritDamage = 0;

        criticalDamage = criticalRate = 0;
        trueDamage = armorPenetration = lifeSteal = attackSpeed = 0;

        maxHealth = maxMana = maxSpirit = 0;
        physicalDefense = magicalDefense = spiritDefense = 0;

        healthRegen = manaRegen = spiritRegen = 0;
        allyHealthRegen = allyManaRegen = allySpiritRegen = 0;

        reduceCritDamage = reduceArmorPen = reduceTrueDamage = 0;
        reflectDamage = moveSpeed = 0;

        immuneAllyDamage = immuneAllyEffects = immuneAllFromAllies = 0;
        cleanseAllyEffects = 0;

        grievousWound = reduceEnemyMana = reduceEnemySpirit = 0;
        weakenTarget = paralyzeChance = rootChance = stunChance = silenceChance = 0;

        immuneDamage = immuneEffects = immuneAll = 0;
        reduceEffectDuration = effectResistance = 0;
    }

    // ============================
    // TABLE (sheet bạn gửi)
    // ============================
    private struct EquipmentAutoRow
    {
        // Lưu dạng % theo sheet (20 = 20%), lúc apply sẽ /100
        public float physPct, magPct, sprPct;
        public float critDmgPct, critRatePct;
        public float trueDmgPct, armorPenPct, lifeStealPct, atkSpeedPct;

        public float maxHpPct, maxManaPct, maxSpiritPct;
        public float physDefPct, magDefPct, sprDefPct;

        public float moveSpeedPct, reflectPct;
    }

    private readonly struct EquipmentAutoKey : IEquatable<EquipmentAutoKey>
    {
        public readonly string name;
        public readonly int level;
        public readonly QualityType quality;

        public EquipmentAutoKey(string name, int level, QualityType quality)
        {
            this.name = NormalizeName(name);
            this.level = level;
            this.quality = quality;
        }

        public bool Equals(EquipmentAutoKey other)
            => name == other.name && level == other.level && quality == other.quality;

        public override bool Equals(object obj)
            => obj is EquipmentAutoKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (name?.GetHashCode() ?? 0);
                hash = hash * 31 + level.GetHashCode();
                hash = hash * 31 + quality.GetHashCode();
                return hash;
            }
        }

        private static string NormalizeName(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Trim().ToLowerInvariant();
        }
    }

    private static float P(float percentInSheet) => percentInSheet / 100f;

    private static Dictionary<EquipmentAutoKey, EquipmentAutoRow> _table;

    private static void EnsureTable()
    {
        if (_table != null) return;

        _table = new Dictionary<EquipmentAutoKey, EquipmentAutoRow>();

        // =========================
        // KIẾM
        // =========================
        _table[new EquipmentAutoKey("Kiếm gỗ", 1, QualityType.Mortal)] = new EquipmentAutoRow
        {
            critDmgPct = 20
        };

        _table[new EquipmentAutoKey("Kiếm đồng", 2, QualityType.Mortal)] = new EquipmentAutoRow
        {
            critDmgPct = 30
        };
        _table[new EquipmentAutoKey("Kiếm đồng", 2, QualityType.Yellow)] = new EquipmentAutoRow
        {
            critDmgPct = 40
        };
        _table[new EquipmentAutoKey("Kiếm đồng", 2, QualityType.Mystic)] = new EquipmentAutoRow
        {
            critDmgPct = 70
        };

        _table[new EquipmentAutoKey("Kiếm bạch ngân", 3, QualityType.Mortal)] = new EquipmentAutoRow
        {
            critDmgPct = 60
        };
        _table[new EquipmentAutoKey("Kiếm bạch ngân", 3, QualityType.Yellow)] = new EquipmentAutoRow
        {
            critDmgPct = 70
        };
        _table[new EquipmentAutoKey("Kiếm bạch ngân", 3, QualityType.Mystic)] = new EquipmentAutoRow
        {
            critDmgPct = 85,
            atkSpeedPct = 7.5f
        };
        _table[new EquipmentAutoKey("Kiếm bạch ngân", 3, QualityType.Earth)] = new EquipmentAutoRow
        {
            critDmgPct = 127.5f,
            atkSpeedPct = 11.25f
        };
        _table[new EquipmentAutoKey("Kiếm bạch ngân", 3, QualityType.Heaven)] = new EquipmentAutoRow
        {
            magPct = 25,
            critDmgPct = 175,
            atkSpeedPct = 25
        };

        // =========================
        // ĐAO
        // =========================
        _table[new EquipmentAutoKey("Đao gỗ", 1, QualityType.Mortal)] = new EquipmentAutoRow
        {
            magPct = 20
        };

        _table[new EquipmentAutoKey("Đao đồng", 2, QualityType.Mortal)] = new EquipmentAutoRow
        {
            magPct = 30
        };
        _table[new EquipmentAutoKey("Đao đồng", 2, QualityType.Yellow)] = new EquipmentAutoRow
        {
            magPct = 40
        };
        _table[new EquipmentAutoKey("Đao đồng", 2, QualityType.Mystic)] = new EquipmentAutoRow
        {
            magPct = 70
        };

        _table[new EquipmentAutoKey("Đao bạch ngân", 3, QualityType.Mortal)] = new EquipmentAutoRow
        {
            magPct = 60
        };
        _table[new EquipmentAutoKey("Đao bạch ngân", 3, QualityType.Yellow)] = new EquipmentAutoRow
        {
            magPct = 70
        };
        _table[new EquipmentAutoKey("Đao bạch ngân", 3, QualityType.Mystic)] = new EquipmentAutoRow
        {
            magPct = 75,
            critRatePct = 5
        };
        _table[new EquipmentAutoKey("Đao bạch ngân", 3, QualityType.Earth)] = new EquipmentAutoRow
        {
            magPct = 112.5f,
            critRatePct = 7.5f
        };
        _table[new EquipmentAutoKey("Đao bạch ngân", 3, QualityType.Heaven)] = new EquipmentAutoRow
        {
            magPct = 100,
            critDmgPct = 75,
            critRatePct = 15
        };

        // =========================
        // THƯƠNG
        // =========================
        _table[new EquipmentAutoKey("Thương gỗ", 1, QualityType.Mortal)] = new EquipmentAutoRow
        {
            armorPenPct = 4
        };

        _table[new EquipmentAutoKey("Thương đồng", 2, QualityType.Mortal)] = new EquipmentAutoRow
        {
            armorPenPct = 6
        };
        _table[new EquipmentAutoKey("Thương đồng", 2, QualityType.Yellow)] = new EquipmentAutoRow
        {
            armorPenPct = 8
        };
        _table[new EquipmentAutoKey("Thương đồng", 2, QualityType.Mystic)] = new EquipmentAutoRow
        {
            armorPenPct = 14
        };

        _table[new EquipmentAutoKey("Thương bạch ngân", 3, QualityType.Mortal)] = new EquipmentAutoRow
        {
            armorPenPct = 12
        };
        _table[new EquipmentAutoKey("Thương bạch ngân", 3, QualityType.Yellow)] = new EquipmentAutoRow
        {
            armorPenPct = 14
        };
        _table[new EquipmentAutoKey("Thương bạch ngân", 3, QualityType.Mystic)] = new EquipmentAutoRow
        {
            physPct = 10,
            armorPenPct = 18
        };
        _table[new EquipmentAutoKey("Thương bạch ngân", 3, QualityType.Earth)] = new EquipmentAutoRow
        {
            physPct = 15,
            armorPenPct = 27
        };
        _table[new EquipmentAutoKey("Thương bạch ngân", 3, QualityType.Heaven)] = new EquipmentAutoRow
        {
            physPct = 25,
            armorPenPct = 35,
            lifeStealPct = 25
        };

        // =========================
        // CUNG
        // =========================
        _table[new EquipmentAutoKey("Cung gỗ", 1, QualityType.Mortal)] = new EquipmentAutoRow
        {
            atkSpeedPct = 10
        };

        _table[new EquipmentAutoKey("Cung đồng", 2, QualityType.Mortal)] = new EquipmentAutoRow
        {
            atkSpeedPct = 15
        };
        _table[new EquipmentAutoKey("Cung đồng", 2, QualityType.Yellow)] = new EquipmentAutoRow
        {
            atkSpeedPct = 20
        };
        _table[new EquipmentAutoKey("Cung đồng", 2, QualityType.Mystic)] = new EquipmentAutoRow
        {
            atkSpeedPct = 35
        };

        _table[new EquipmentAutoKey("Cung bạch ngân", 3, QualityType.Mortal)] = new EquipmentAutoRow
        {
            atkSpeedPct = 30
        };
        _table[new EquipmentAutoKey("Cung bạch ngân", 3, QualityType.Yellow)] = new EquipmentAutoRow
        {
            atkSpeedPct = 35
        };
        _table[new EquipmentAutoKey("Cung bạch ngân", 3, QualityType.Mystic)] = new EquipmentAutoRow
        {
            magPct = 10,
            atkSpeedPct = 45
        };
        _table[new EquipmentAutoKey("Cung bạch ngân", 3, QualityType.Earth)] = new EquipmentAutoRow
        {
            magPct = 15,
            atkSpeedPct = 67.5f
        };
        _table[new EquipmentAutoKey("Cung bạch ngân", 3, QualityType.Heaven)] = new EquipmentAutoRow
        {
            magPct = 25,
            critRatePct = 10,
            atkSpeedPct = 87.5f
        };
    }

    private bool TryApplyFromTable(string equipmentName, int level, QualityType quality)
    {
        EnsureTable();

        var key = new EquipmentAutoKey(equipmentName, level, quality);
        if (!_table.TryGetValue(key, out var row))
            return false;

        // Apply: convert % -> decimal
        physicalDamage = P(row.physPct);
        magicalDamage = P(row.magPct);
        spiritDamage = P(row.sprPct);

        criticalDamage = P(row.critDmgPct);
        criticalRate = P(row.critRatePct);

        trueDamage = P(row.trueDmgPct);
        armorPenetration = P(row.armorPenPct);
        lifeSteal = P(row.lifeStealPct);
        attackSpeed = P(row.atkSpeedPct);

        maxHealth = P(row.maxHpPct);
        maxMana = P(row.maxManaPct);
        maxSpirit = P(row.maxSpiritPct);

        physicalDefense = P(row.physDefPct);
        magicalDefense = P(row.magDefPct);
        spiritDefense = P(row.sprDefPct);

        moveSpeed = P(row.moveSpeedPct);
        reflectDamage = P(row.reflectPct);

        return true;
    }

    public override ItemData GetItemData()
    {
        ItemData data = base.GetItemData();

        return new EquipmentData
        {
            // base
            instanceId = data.instanceId,
            itemId = data.itemId,
            itemName = data.itemName,
            itemType = data.itemType,
            itemIcon = data.itemIcon,
            itemDescription = data.itemDescription,
            itemIconPath = data.itemIconPath,
            itemPrice = data.itemPrice,
            currentstack = data.currentstack,

            // equipment meta
            equipmentType = equipmentType,
            level = level,
            qualityType = qualityType,
            raceType = raceType,
            elementType = elementType,

            // damage
            physicalDamage = physicalDamage,
            magicalDamage = magicalDamage,
            spiritDamage = spiritDamage,
            critDamage = criticalDamage,
            critRate = criticalRate,
            trueDamage = trueDamage,
            armorPenetration = armorPenetration,
            lifeSteal = lifeSteal,
            attackSpeed = attackSpeed,

            // defense
            maxHealth = maxHealth,
            maxMana = maxMana,
            maxSpirit = maxSpirit,
            physicalDefense = physicalDefense,
            magicalDefense = magicalDefense,
            spiritDefense = spiritDefense,
            healthRegen = healthRegen,
            manaRegen = manaRegen,
            spiritRegen = spiritRegen,
            allyHealthRegen = allyHealthRegen,
            allyManaRegen = allyManaRegen,
            allySpiritRegen = allySpiritRegen,
            critDamageReduction = reduceCritDamage,
            reflectDamage = reflectDamage,
            moveSpeed = moveSpeed,

            // effects
            immuneAllyDamage = immuneAllyDamage,
            immuneAllyEffects = immuneAllyEffects,
            immuneAllFromAllies = immuneAllFromAllies,
            cleanseAllyEffects = cleanseAllyEffects,
            grievousWound = grievousWound,
            reduceEnemyMana = reduceEnemyMana,
            reduceEnemySpirit = reduceEnemySpirit,
            weakenTarget = weakenTarget,
            paralyzeChance = paralyzeChance,
            rootChance = rootChance,
            stunChance = stunChance,
            silenceChance = silenceChance,
            immuneDamage = immuneDamage,
            immuneEffects = immuneEffects,
            immuneAll = immuneAll,
            reduceEffectDuration = reduceEffectDuration,
            effectResistance = effectResistance
        };
    }
}