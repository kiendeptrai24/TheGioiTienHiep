public sealed class CooldownReadyCondition : ISkillCondition
{
    public ConditionResult Evaluate(in SkillContext ctx)
    {
        return ctx.Runtime.IsReady(ctx.Time.Now)
            ? ConditionResult.Pass()
            : ConditionResult.Fail(SkillCastFailReason.OnCooldown);
    }
}
