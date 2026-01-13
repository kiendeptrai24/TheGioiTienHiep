

using System;
using UnityEngine;

public class IdentifySkill : BaseSkill
{
    public IdentifySkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f)
        : base(data, skillEffectPrefab, skillId, displayName, cooldownSeconds)
    {
    }

    protected override void OnApplyEffect(in SkillContext ctx)
    {
        // Vector3 spawnPos =
        // ctx.Caster.Position.position +
        // ctx.Caster.transform.forward * 1.5f;

        Vector3 targetPosition = ctx.TargetDirection.position;
        Quaternion targetRotation = ctx.TargetDirection.rotation;

        var fireball = GameObject.Instantiate(skillEffectPrefab, targetPosition, targetRotation);
        var projectile = fireball.GetComponent<ParticleSystem>();
        StatsData statsData = ctx.Caster.GetStats();

        if (projectile != null)
        {
            projectile.Play();
            Collider[] colliders = Physics.OverlapSphere(
                targetPosition,
                data.attackRange
            );
            
            foreach (Collider col in colliders)
            {
                if(col.TryGetComponent<ISkillCaster>(out var caster))
                {
                    if(caster.TeamId == ctx.Caster.TeamId)
                    {
                        continue;
                    }
                }
                if (col.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(statsData);
                }
            }
        }
        GameObject.Destroy(fireball, 1f);
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}