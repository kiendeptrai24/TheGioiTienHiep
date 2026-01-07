

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
        if (projectile != null)
            projectile.Play();
        GameObject.Destroy(fireball, 1f);
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}