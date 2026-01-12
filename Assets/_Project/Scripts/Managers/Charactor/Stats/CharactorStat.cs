
using System.Collections;
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

    public virtual void TakeDamage(SkillContext ctx, StatsData _targetStats)
    {
        var heal = GetComponent<HealthController>();
        if (heal != null)
        {
            heal.DecreaseHealth(stats.MagicalDamage, ctx.Caster.Id);
        }
    }
}

