

using System;
using UnityEngine;

public class FireballSkill : BaseSkill
{
    public FireballSkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f)
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
        {
            Debug.Log("Fireball launched towards target at position: " + targetPosition);
            projectile.Play();
        }
        GameObject.Destroy(fireball, 1f);
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}