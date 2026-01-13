

using System;
using Unity.Netcode;
using UnityEngine;

public class IdentifySkill : BaseSkill
{
    public IdentifySkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f)
        : base(data, skillEffectPrefab, skillId, displayName, cooldownSeconds)
    {
    }

    protected override void OnApplyEffect(in SkillContext ctx)
    {

        Vector3 targetPosition = ctx.TargetDirection.position;
        Quaternion targetRotation = ctx.TargetDirection.rotation;

        var skillEffect = NetworkObjectPool.Singleton.GetNetworkObject(skillEffectPrefab, targetPosition, targetRotation, true);
        var networkObject = skillEffect.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject);
        }
        StatsData statsData = ctx.Caster.GetStats();

        var partical = skillEffect.GetComponent<ParticleSystem>();
        if (partical != null)
        {
            partical.Play();
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
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}