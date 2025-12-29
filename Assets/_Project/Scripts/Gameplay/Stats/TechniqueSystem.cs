
using TGTH.Mobile;
using UnityEngine;

public class TechniqueSystem : StatsSystem
{
    [SerializeField] private TechniquePresenter techniquePresenter;
    protected override void Awake()
    {
        base.Awake();
        techniquePresenter?.SetEquipmentSystem(this);
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
    }

    public override void Equip(InventoryItem item)
    {
        if (item == null) return;
        if (item.data is not TechniqueData data) return;

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

        // ===== DEFENSE =====
        AddPercent(StatType.PhysicalDefense, data.physicalDefense);
        AddPercent(StatType.MagicalDefense, data.magicalDefense);
        AddPercent(StatType.SpiritDefense, data.spiritDefense);

        AddPercent(StatType.CritDamageReduction, data.critDamageReduction);
        AddPercent(StatType.PenetrationDamageReduction, data.penetrationReduction);
        AddPercent(StatType.TrueDamageReduction, data.trueDamageReduction);

        AddPercent(StatType.BonusHealth, data.bonusHealth);
        AddPercent(StatType.BonusMana, data.bonusMana);
        AddPercent(StatType.BonusSpirit, data.bonusSpirit);
    }

    public override void Unequip(InventoryItem item)
    {
        if (item == null) return;
        if (item.data is not TechniqueData data) return;

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

        // ===== DEFENSE =====
        RemovePercent(StatType.PhysicalDefense, data.physicalDefense);
        RemovePercent(StatType.MagicalDefense, data.magicalDefense);
        RemovePercent(StatType.SpiritDefense, data.spiritDefense);

        RemovePercent(StatType.CritDamageReduction, data.critDamageReduction);
        RemovePercent(StatType.PenetrationDamageReduction, data.penetrationReduction);
        RemovePercent(StatType.TrueDamageReduction, data.trueDamageReduction);

        RemovePercent(StatType.BonusHealth, data.bonusHealth);
        RemovePercent(StatType.BonusMana, data.bonusMana);
        RemovePercent(StatType.BonusSpirit, data.bonusSpirit);
    }
}