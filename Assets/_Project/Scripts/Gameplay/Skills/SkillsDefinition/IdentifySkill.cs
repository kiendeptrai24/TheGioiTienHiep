

using System;
using Unity.Netcode;
using UnityEngine;

public class IdentifySkill : BaseSkill
{
    public IdentifySkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f, Type skillAnimationClass = null)
        : base(data, skillEffectPrefab, skillId, displayName, cooldownSeconds, skillAnimationClass)
    {
    }

    protected override void OnApplyEffect(in SkillContext ctx)
    {

        Vector3 targetPosition = ctx.TargetDirection.position;
        Quaternion targetRotation = ctx.TargetDirection.rotation;
        var skillEffect = ObjectPool.Instance.GetObject(skillEffectPrefab, targetPosition, targetRotation);
        ObjectPool.Instance.ReturnObject(skillEffect, 1f);

        var partical = skillEffect.GetComponent<ParticleSystem>();
        if (partical != null)
        {
            partical.Play();
        }
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}