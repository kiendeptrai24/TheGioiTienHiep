

using UnityEngine;

public class FireballSkill : BaseSkill
{
    public FireballSkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f)
        : base(data, skillEffectPrefab, skillId, displayName, cooldownSeconds)
    {
    }

    protected override void OnApplyEffect(in SkillContext ctx)
    {
        
    }
    override public void BuildDefaultConditions()
    {
        base.BuildDefaultConditions();
    }
}