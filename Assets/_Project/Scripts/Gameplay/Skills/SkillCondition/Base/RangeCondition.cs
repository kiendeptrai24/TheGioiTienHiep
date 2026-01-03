public sealed class RangeCondition : ISkillCondition
{
    private readonly float _range;

    public RangeCondition(float range) => _range = range;

    public ConditionResult Evaluate(in SkillContext ctx)
    {
        if (ctx.Target == null) return ConditionResult.Fail(SkillCastFailReason.NoTarget);
        float d = UnityEngine.Vector3.Distance(ctx.Caster.Position, ctx.Target.Position);
        return d <= _range ? ConditionResult.Pass() : ConditionResult.Fail(SkillCastFailReason.OutOfRange);
    }
}
