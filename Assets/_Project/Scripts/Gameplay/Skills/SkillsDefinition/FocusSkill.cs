

using System;
using UnityEngine;

public class FocusSkill : BaseSkill
{
    public FocusSkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f)
        : base(data, skillEffectPrefab, skillId, displayName, cooldownSeconds)
    {
    }

    protected override void OnApplyEffect(in SkillContext ctx)
    {

        Vector3 targetPosition = ctx.Target.Center;
        Quaternion targetRotation = ctx.Target.Rotation;

        var fireball = GameObject.Instantiate(skillEffectPrefab, targetPosition, targetRotation);
        var projectile = fireball.GetComponent<ParticleSystem>();
        if (projectile != null)
        {
            Debug.Log("Focus launched towards target at position: " + targetPosition);
            projectile.Play();
        }
        GameObject.Destroy(fireball, 1f);
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}