using System;
using UnityEngine;

public abstract class BaseSkill
{
    private string _skillId;
    private string _displayName;
    private float _cooldownSeconds = 1;
    public SkillData data;
    public GameObject skillEffectPrefab;

    public string SkillId => _skillId;
    public string DisplayName => _displayName;
    public float CooldownSeconds => _cooldownSeconds;

    /// Chi phí (có thể tách hẳn thành Cost system nếu bạn muốn)
    public float ManaCost { get; protected set; } = 0f;

    /// Root condition để quyết định có cast được không
    protected ISkillCondition RootCondition;

    protected BaseSkill(SkillData data, GameObject skillEffectPrefab, string skillId, string displayName, float cooldownSeconds = 1f)
    {
        this.data = data;
        this.skillEffectPrefab = skillEffectPrefab;
        _skillId = skillId;
        _displayName = displayName;
        _cooldownSeconds = cooldownSeconds;
    }

    /// Setup điều kiện mặc định. Skill con có thể override và build condition theo ý.
    public virtual void BuildDefaultConditions()
    {
        RootCondition = new AllOfCondition()
            .Add(new CooldownReadyCondition())
            .Add(new ManaCondition(ManaCost));
    }

    public ConditionResult CanCast(in SkillContext ctx)
    {
        if (RootCondition == null) return ConditionResult.Pass();
        return RootCondition.Evaluate(ctx);
    }

    /// Gọi khi cast thành công: trừ resource, set cooldown, apply effect…
    public void OnCastSucceeded(in SkillContext ctx)
    {
        if (ManaCost > 0f) ctx.Caster.ConsumeMana(ManaCost);
        ctx.Runtime.TriggerCooldown(ctx.Time.Now, CooldownSeconds);
        OnApplyEffect(ctx);
    }

    /// Skill con override để gây sát thương / buff / spawn projectile…
    protected abstract void OnApplyEffect(in SkillContext ctx);
}