
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

    public virtual void TakeDamage(StatsData _casterStats)
    {
        var heal = GetComponent<HealthController>();
        if (_casterStats == null)
            return;
        var Caster = _casterStats.GetComponent<HeroController>();
        if (heal != null && Caster != null)
        {
            heal.DecreaseHealth(_casterStats.PhysicalDamage, Caster.Id);
        }
    }
}

