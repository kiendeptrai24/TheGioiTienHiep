public sealed class HasTargetCondition : ISkillCondition
{
    public ConditionResult Evaluate(in SkillContext ctx)
    {
        if (ctx.Target == null) return ConditionResult.Fail(SkillCastFailReason.NoTarget);
        if (!ctx.Target.IsAlive) return ConditionResult.Fail(SkillCastFailReason.TargetDead);
        return ConditionResult.Pass();
    }
}