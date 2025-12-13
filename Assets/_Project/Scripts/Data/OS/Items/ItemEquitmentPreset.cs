using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewEquipmentPreset", menuName = "RPG/Items/Equipment Preset")]
public class ItemEquipmentPreset : ItemPreset
{
    [Header("Equipment Type")]
    public EquipmentType equipmentType;

    [Header("Resources")]
    public Stat health;
    public Stat mana;
    public Stat spirit;

    [Header("Offensive Stats")]
    public Stat physicalDamage;
    public Stat magicalDamage;
    public Stat spiritDamage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive Stats")]
    public Stat physicalDefense;
    public Stat magicalDefense;
    public Stat spiritDefense;
    public Stat evasion;
    public Stat spiritPenetration;

    [Header("Speed Stats")]
    public Stat movementSpeed;
    public Stat attackSpeed;
    public Stat castSpeed;

    [Header("Progression Stats")]
    public Stat potential;
    public Stat skillPoints;
    public Stat combatPower;

    [Header("Critical Stats")]
    public Stat spiritRange;

#if UNITY_EDITOR
    private void OnValidate()
    {
        string typeName = equipmentType.ToString();
        if (!string.IsNullOrEmpty(typeName))
        {
            string newName = $"Equip_{typeName}";
            string path = AssetDatabase.GetAssetPath(this);

            if (!string.IsNullOrEmpty(path) && name != newName)
            {
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();
            }
        }

        ResetToDefault();
    }
#endif

    [ContextMenu("Reset To Default")]
    public void ResetToDefault()
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                ApplyBaseStats(
                    hp: 0, mp: 0, sp: 0,
                    physDmg: 20, magDmg: 20, sprDmg: 20,
                    physDef: 0, magDef: 0, sprDef: 0,
                    mvSpeed: 0, crit: 5, critPow: 150,
                    evasion: 0, atkSpeed: 3, castSpeed: 2,
                    potential: 0, skillPts: 0, power: 30,
                    sprRange: 0, spiritPen: 0, mindPen: 0
                );
                break;

            case EquipmentType.Armor:
                ApplyBaseStats(
                    hp: 200, mp: 50, sp: 50,
                    physDmg: 0, magDmg: 0, sprDmg: 0,
                    physDef: 15, magDef: 15, sprDef: 10,
                    mvSpeed: -2, crit: 0, critPow: 0,
                    evasion: 1, atkSpeed: 0, castSpeed: 0,
                    potential: 0, skillPts: 0, power: 20,
                    sprRange: 0, spiritPen: 0, mindPen: 0
                );
                break;

            case EquipmentType.Helmet:
                ApplyBaseStats(
                    hp: 100, mp: 30, sp: 30,
                    physDmg: 0, magDmg: 0, sprDmg: 0,
                    physDef: 10, magDef: 10, sprDef: 10,
                    mvSpeed: 0, crit: 2, critPow: 20,
                    evasion: 1, atkSpeed: 0, castSpeed: 0,
                    potential: 0, skillPts: 0, power: 10,
                    sprRange: 0, spiritPen: 0, mindPen: 0
                );
                break;

            case EquipmentType.Pants:
                ApplyBaseStats(
                    hp: 120, mp: 20, sp: 20,
                    physDmg: 0, magDmg: 0, sprDmg: 0,
                    physDef: 10, magDef: 10, sprDef: 10,
                    mvSpeed: 1, crit: 0, critPow: 0,
                    evasion: 2, atkSpeed: 0, castSpeed: 0,
                    potential: 0, skillPts: 0, power: 10,
                    sprRange: 0, spiritPen: 0, mindPen: 0
                );
                break;

            case EquipmentType.Boots:
                ApplyBaseStats(
                    hp: 50, mp: 0, sp: 0,
                    physDmg: 0, magDmg: 0, sprDmg: 0,
                    physDef: 5, magDef: 5, sprDef: 5,
                    mvSpeed: 5, crit: 1, critPow: 10,
                    evasion: 3, atkSpeed: 1, castSpeed: 0,
                    potential: 0, skillPts: 0, power: 15,
                    sprRange: 0, spiritPen: 0, mindPen: 0
                );
                break;

            case EquipmentType.Belt:
                ApplyBaseStats(
                    hp: 80, mp: 50, sp: 50,
                    physDmg: 0, magDmg: 0, sprDmg: 0,
                    physDef: 5, magDef: 5, sprDef: 5,
                    mvSpeed: 0, crit: 0, critPow: 0,
                    evasion: 0, atkSpeed: 0, castSpeed: 0,
                    potential: 20, skillPts: 5, power: 15,
                    sprRange: 0, spiritPen: 0, mindPen: 0
                );
                break;

            case EquipmentType.Ring:
                ApplyBaseStats(
                    hp: 0, mp: 20, sp: 20,
                    physDmg: 10, magDmg: 10, sprDmg: 10,
                    physDef: 0, magDef: 0, sprDef: 0,
                    mvSpeed: 0, crit: 3, critPow: 30,
                    evasion: 0, atkSpeed: 1, castSpeed: 1,
                    potential: 0, skillPts: 0, power: 20,
                    sprRange: 5, spiritPen: 5, mindPen: 5
                );
                break;

            case EquipmentType.Necklace:
                ApplyBaseStats(
                    hp: 0, mp: 40, sp: 40,
                    physDmg: 5, magDmg: 5, sprDmg: 5,
                    physDef: 0, magDef: 0, sprDef: 0,
                    mvSpeed: 0, crit: 1, critPow: 10,
                    evasion: 0, atkSpeed: 0, castSpeed: 2,
                    potential: 0, skillPts: 0, power: 25,
                    sprRange: 10, spiritPen: 10, mindPen: 10
                );
                break;

            case EquipmentType.Bracelet:
                ApplyBaseStats(
                    hp: 0, mp: 30, sp: 30,
                    physDmg: 8, magDmg: 8, sprDmg: 8,
                    physDef: 0, magDef: 0, sprDef: 0,
                    mvSpeed: 0, crit: 2, critPow: 20,
                    evasion: 0, atkSpeed: 1, castSpeed: 1,
                    potential: 0, skillPts: 0, power: 20,
                    sprRange: 8, spiritPen: 8, mindPen: 8
                );
                break;
        }
    }

    private void ApplyBaseStats(
        float hp, float mp, float sp,
        float physDmg, float magDmg, float sprDmg,
        float physDef, float magDef, float sprDef,
        float mvSpeed, float crit, float critPow,
        float evasion, float atkSpeed, float castSpeed,
        float potential, float skillPts, float power,
        float sprRange, float spiritPen, float mindPen
    )
    {
        health = new Stat(StatType.Health, hp);
        mana = new Stat(StatType.Mana, mp);
        spirit = new Stat(StatType.Spirit, sp);

        physicalDamage = new Stat(StatType.PhysicalDamage, physDmg);
        magicalDamage = new Stat(StatType.MagicalDamage, magDmg);
        spiritDamage = new Stat(StatType.SpiritDamage, sprDmg);

        physicalDefense = new Stat(StatType.PhysicalDefense, physDef);
        magicalDefense = new Stat(StatType.MagicalDefense, magDef);
        spiritDefense = new Stat(StatType.SpiritDefense, sprDef);

        movementSpeed = new Stat(StatType.MovementSpeed, mvSpeed);
        critChance = new Stat(StatType.CritChance, crit);
        critPower = new Stat(StatType.CritPower, critPow);

        this.evasion = new Stat(StatType.Evasion, evasion);
        attackSpeed = new Stat(StatType.AttackSpeed, atkSpeed);
        this.castSpeed = new Stat(StatType.CastSpeed, castSpeed);

        this.potential = new Stat(StatType.Potential, potential);
        skillPoints = new Stat(StatType.SkillPoints, skillPts);
        combatPower = new Stat(StatType.CombatPower, power);

        spiritRange = new Stat(StatType.SpiritRange, sprRange);
        spiritPenetration = new Stat(StatType.SpiritPenetration, spiritPen);
    }
}
