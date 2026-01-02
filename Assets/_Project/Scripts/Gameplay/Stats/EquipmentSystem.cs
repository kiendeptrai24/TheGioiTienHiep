using TGTH.Mobile;
using UnityEngine;

public class EquipmentSystem : StatsSystem
{
    // private CharacterStats charStats;
    [SerializeField] private EquipmentPresenter equipmentPresenter;
    protected override void Awake()
    {
        base.Awake();
        equipmentPresenter?.SetEquipmentSystem(this);
    }
    public override void Equip(InventoryItem item)
    {
        if (item == null) return;
        if (item.data is not EquitmentData data) return;
        // ===== DAMAGE =====
        AddPercent(StatType.PhysicalDamage, data.physicalDamage);
        AddPercent(StatType.MagicalDamage, data.magicalDamage);
        AddPercent(StatType.SpiritDamage, data.spiritDamage);

        AddPercent(StatType.CritPower, data.critDamage);
        AddPercent(StatType.CritChance, data.critRate);

        AddPercent(StatType.TrueDamage, data.trueDamage);
        AddPercent(StatType.ArmorPenetration, data.armorPenetration);
        AddPercent(StatType.LifeSteal, data.lifeSteal);

        AddPercent(StatType.AttackSpeed, data.attackSpeed);

        // ===== DEFENSE ====
        AddPercent(StatType.PhysicalDefense, data.physicalDefense);
        AddPercent(StatType.MagicalDefense, data.magicalDefense);
        AddPercent(StatType.SpiritDefense, data.spiritDefense);

        // ===== RESOURCE =====
        AddPercent(StatType.Health, data.maxHealth);
        AddPercent(StatType.Mana, data.maxMana);
        AddPercent(StatType.Spirit, data.maxSpirit);

        // ===== REGEN =====
        AddPercent(StatType.HealthRegen, data.healthRegen);
        AddPercent(StatType.ManaRegen, data.manaRegen);
        AddPercent(StatType.SpiritRegen, data.spiritRegen);

        AddPercent(StatType.AllyHealthRegen, data.allyHealthRegen);
        AddPercent(StatType.AllyManaRegen, data.allyManaRegen);
        AddPercent(StatType.AllySpiritRegen, data.allySpiritRegen);

        // ===== DEFENSE REDUCTION =====
        AddPercent(StatType.CritDamageReduction, data.critDamageReduction);
        AddPercent(StatType.PenetrationDamageReduction, data.armorPenetrationReduction);
        AddPercent(StatType.TrueDamageReduction, data.trueDamageReduction);

        AddPercent(StatType.ReflectDamage, data.reflectDamage);
        AddPercent(StatType.MovementSpeed, data.moveSpeed);

        // ===== IMMUNITY =====
        AddPercent(StatType.AllyDamageImmunity, data.immuneAllyDamage);
        AddPercent(StatType.AllyCCImmunity, data.immuneAllyEffects);
        AddPercent(StatType.AllyFullImmunity, data.immuneAllFromAllies);

        AddPercent(StatType.DamageImmunity, data.immuneDamage);
        AddPercent(StatType.CCImmunity, data.immuneEffects);
        AddPercent(StatType.FullImmunity, data.immuneAll);

        // ===== DEBUFF =====
        AddPercent(StatType.HealingReduction, data.grievousWound);
        AddPercent(StatType.EnemyManaReduction, data.reduceEnemyMana);
        AddPercent(StatType.EnemySpiritReduction, data.reduceEnemySpirit);

        // ===== CC =====
        AddPercent(StatType.Weaken, data.weakenTarget);
        AddPercent(StatType.Paralyze, data.paralyzeChance);
        AddPercent(StatType.Root, data.rootChance);
        AddPercent(StatType.Stun, data.stunChance);
        AddPercent(StatType.Silence, data.silenceChance);

        // ===== RESIST =====
        AddPercent(StatType.CCDurationReduction, data.reduceEffectDuration);
        AddPercent(StatType.CCResistance, data.effectResistance);
    }
    public override void Unequip(InventoryItem item)
    {
        if (item == null)
            return;

        if (item.data is not EquitmentData data) return;

        // ===== DAMAGE =====
        RemovePercent(StatType.PhysicalDamage, data.physicalDamage);
        RemovePercent(StatType.MagicalDamage, data.magicalDamage);
        RemovePercent(StatType.SpiritDamage, data.spiritDamage);

        RemovePercent(StatType.CritPower, data.critDamage);
        RemovePercent(StatType.CritChance, data.critRate);

        RemovePercent(StatType.TrueDamage, data.trueDamage);
        RemovePercent(StatType.ArmorPenetration, data.armorPenetration);
        RemovePercent(StatType.LifeSteal, data.lifeSteal);

        RemovePercent(StatType.AttackSpeed, data.attackSpeed);

        // ===== DEFENSE ====
        RemovePercent(StatType.PhysicalDefense, data.physicalDefense);
        RemovePercent(StatType.MagicalDefense, data.magicalDefense);
        RemovePercent(StatType.SpiritDefense, data.spiritDefense);

        // ===== RESOURCE =====
        RemovePercent(StatType.Health, data.maxHealth);
        RemovePercent(StatType.Mana, data.maxMana);
        RemovePercent(StatType.Spirit, data.maxSpirit);

        // ===== REGEN =====
        RemovePercent(StatType.HealthRegen, data.healthRegen);
        RemovePercent(StatType.ManaRegen, data.manaRegen);
        RemovePercent(StatType.SpiritRegen, data.spiritRegen);

        RemovePercent(StatType.AllyHealthRegen, data.allyHealthRegen);
        RemovePercent(StatType.AllyManaRegen, data.allyManaRegen);
        RemovePercent(StatType.AllySpiritRegen, data.allySpiritRegen);

        // ===== DEFENSE REDUCTION =====
        RemovePercent(StatType.CritDamageReduction, data.critDamageReduction);
        RemovePercent(StatType.PenetrationDamageReduction, data.armorPenetrationReduction);
        RemovePercent(StatType.TrueDamageReduction, data.trueDamageReduction);

        AddPercent(StatType.ReflectDamage, data.reflectDamage);
        AddPercent(StatType.MovementSpeed, data.moveSpeed);

        // ===== IMMUNITY =====
        RemovePercent(StatType.AllyDamageImmunity, data.immuneAllyDamage);
        RemovePercent(StatType.AllyCCImmunity, data.immuneAllyEffects);
        RemovePercent(StatType.AllyFullImmunity, data.immuneAllFromAllies);

        RemovePercent(StatType.DamageImmunity, data.immuneDamage);
        RemovePercent(StatType.CCImmunity, data.immuneEffects);
        RemovePercent(StatType.FullImmunity, data.immuneAll);

        // ===== DEBUFF =====
        RemovePercent(StatType.HealingReduction, data.grievousWound);
        RemovePercent(StatType.EnemyManaReduction, data.reduceEnemyMana);
        RemovePercent(StatType.EnemySpiritReduction, data.reduceEnemySpirit);

        // ===== CC =====
        RemovePercent(StatType.Weaken, data.weakenTarget);
        RemovePercent(StatType.Paralyze, data.paralyzeChance);
        RemovePercent(StatType.Root, data.rootChance);
        RemovePercent(StatType.Stun, data.stunChance);
        RemovePercent(StatType.Silence, data.silenceChance);

        // ===== RESIST =====
        RemovePercent(StatType.CCDurationReduction, data.reduceEffectDuration);
        RemovePercent(StatType.CCResistance, data.effectResistance);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}
