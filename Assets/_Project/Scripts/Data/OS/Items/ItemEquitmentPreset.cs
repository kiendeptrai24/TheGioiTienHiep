using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentPreset", menuName = "RPG/Items/Equipment Preset")]
public class ItemEquipmentPreset : ItemStatsPreset
{
    [Header("Equipment Type")]
    public EquipmentType equipmentType;
    public int level;
    public RaceType raceType;
    public ElementType elementType;

    [Header("Damage Stats")]
    public float criticalDamage;
    public float criticalRate;

    public float trueDamage;
    public float armorPenetration;
    public float lifeSteal;
    public float attackSpeed;

    [Header("Defense Stats")]
    public float maxHealth;
    public float maxMana;
    public float maxSpirit;

    public float healthRegen;
    public float manaRegen;
    public float spiritRegen;

    public float allyHealthRegen;
    public float allyManaRegen;
    public float allySpiritRegen;

    public float reduceCritDamage;
    public float reduceArmorPen;
    public float reduceTrueDamage;

    public float reflectDamage;
    public float moveSpeed;

    [Header("Effect & Immunity")]
    public float immuneAllyDamage;
    public float immuneAllyEffects;
    public float immuneAllFromAllies;

    public float cleanseAllyEffects;

    public float grievousWound;
    public float reduceEnemyMana;
    public float reduceEnemySpirit;

    public float weakenTarget;
    public float paralyzeChance;
    public float rootChance;
    public float stunChance;
    public float silenceChance;

    public float immuneDamage;
    public float immuneEffects;
    public float immuneAll;

    public float reduceEffectDuration;
    public float effectResistance;


    public override ItemData GetItemData()
    {
        ItemData data = base.GetItemData();

        return new EquitmentData
        {
            // base
            itemId = data.itemId,
            itemName = data.itemName,
            itemType = data.itemType,
            itemIcon = data.itemIcon,
            itemDescription = data.itemDescription,
            currentstack = data.currentstack,
            canStack = data.canStack,
            itemPrice = data.itemPrice,
            realmType = data.realmType,
            qualityType = data.qualityType,

            // equipment meta
            equipmentType = equipmentType,
            level = level,
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
            armorPenetrationReduction = reduceArmorPen,
            trueDamageReduction = reduceTrueDamage,
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
