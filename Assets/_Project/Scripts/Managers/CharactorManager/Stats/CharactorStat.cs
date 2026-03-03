
using UnityEngine;

public abstract class CharacterStats : TGTHNetworkBehaviour, IDamageable
{
    private StatsData stats;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        stats = GetComponent<StatsData>();
    }

    /// <summary>
    /// Tính toán damage cuối cùng dựa trên stats của người đánh và người bị đánh
    /// </summary>
    protected virtual (int damage, bool isCrit, float lifeSteal, float reflect)
    CalculateDamage(StatsData attacker, StatsData defender)
    {
        if (attacker == null || defender == null) return (0, false, 0, 0);

        // Damage sources
        int physicalDmg = attacker.PhysicalDamage;
        int magicalDmg = attacker.MagicalDamage;
        int spiritDmg = attacker.SpiritDamage;
        float trueDmg = attacker.TrueDamage;

        // Penetration
        float armorPen = attacker.ArmorPenetration;
        float spiritPen = attacker.SpiritPenetration;

        // Defense
        int physicalDef = defender.PhysicalDefense;
        int magicalDef = defender.MagicalDefense;
        int spiritDef = defender.SpiritDefense;

        // Crit
        float critChance = attacker.GetStatValue(StatType.CritChance) / 100f;
        float critPower = 2f + attacker.GetStatValue(StatType.CritPower) / 100f; // 200% + bonus
        float critReduction = defender.CritDamageReduction;

        // Damage reduction
        float penReduction = defender.PenetrationDamageReduction;
        float trueDmgReduction = defender.TrueDamageReduction;

        // Immunity
        float damageImmunity = defender.DamageImmunity;
        if (damageImmunity >= 1f) return (0, false, 0, 0);

        // Calculate damage
        float finalPhysicalDef = Mathf.Max(0, physicalDef - physicalDef * armorPen);
        float finalPhysical = Mathf.Max(0, physicalDmg - finalPhysicalDef);

        float finalMagicalDef = Mathf.Max(0, magicalDef - magicalDef * armorPen);
        float finalMagical = Mathf.Max(0, magicalDmg - finalMagicalDef);

        float finalSpiritDef = Mathf.Max(0, spiritDef - spiritDef * spiritPen);
        float finalSpirit = Mathf.Max(0, spiritDmg - finalSpiritDef);

        float totalDmg = finalPhysical + finalMagical + finalSpirit;

        // Crit
        bool isCrit = Random.value < critChance;
        if (isCrit)
        {
            totalDmg *= critPower * (1f - critReduction);
            Debug.Log("Crit!");
        }

        // Penetration reduction
        totalDmg *= (1f - penReduction);

        // True damage
        totalDmg += trueDmg * (1f - trueDmgReduction);

        // Damage immunity
        totalDmg *= (1f - damageImmunity);

        // LifeSteal
        float lifeSteal = attacker.LifeSteal * totalDmg;

        // Reflect
        float reflect = defender.ReflectDamage * totalDmg;

        // Clamp
        int final = Mathf.Max(0, Mathf.RoundToInt(totalDmg));
        return (final, isCrit, lifeSteal, reflect);
    }

    public virtual void TakeDamage(StatsData _casterStats)
    {
        if (!IsServer) return;
        var heal = GetComponent<HealthController>();
        if (_casterStats == null || stats == null)
            return;
        var Caster = _casterStats.GetComponent<HeroController>();
        if (heal != null && Caster != null)
        {
            var (finalDamage, isCrit, lifeSteal, reflect) = CalculateDamage(_casterStats, stats);
            Debug.Log("Damage: " + finalDamage);
            heal.DecreaseHealth(finalDamage, Caster.Id);

            // Hút máu cho attacker
            if (lifeSteal > 0)
            {
                var attackerHealth = _casterStats.GetComponent<HealthController>();
                if (attackerHealth != null)
                {
                    int healTimes = Mathf.RoundToInt(lifeSteal);
                    for (int i = 0; i < healTimes; i++) attackerHealth.IncreaseHealth();
                }
            }

            // Phản đòn cho attacker
            if (reflect > 0)
            {
                var attackerHealth = _casterStats.GetComponent<HealthController>();
                if (attackerHealth != null)
                {
                    attackerHealth.DecreaseHealth(Mathf.RoundToInt(reflect), Caster.Id);
                }
            }

            // Có thể thêm các hiệu ứng khác ở đây (ví dụ: ghi log chí mạng, hiệu ứng animation...)
        }
    }
}

