
using System;
using Unity.Netcode;
using UnityEngine;

public class FocusSkill : BaseSkill
{
    public FocusSkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f, Type skillAnimationClass = null)
        : base(data, skillEffectPrefab, skillId, displayName, cooldownSeconds, skillAnimationClass)
    {
    }

    protected override void OnApplyEffect(in SkillContext ctx)
    {
        Vector3 targetPosition = ctx.Target.Center;
        Quaternion targetRotation = ctx.Target.Rotation;

        var skillEffect = ObjectPool.Instance.GetObject(skillEffectPrefab, targetPosition, targetRotation);
        ObjectPool.Instance.ReturnObject(skillEffect, 1f);

        var partical = skillEffect.GetComponent<ParticleSystem>();
        if (partical != null)
            partical.Play();
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}