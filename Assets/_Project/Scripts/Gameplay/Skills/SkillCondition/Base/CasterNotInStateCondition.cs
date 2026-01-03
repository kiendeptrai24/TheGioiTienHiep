public sealed class CasterNotInStateCondition : ISkillCondition
{
    private readonly string _blockedStateId;
    public CasterNotInStateCondition(string blockedStateId) => _blockedStateId = blockedStateId;

    public ConditionResult Evaluate(in SkillContext ctx)
    {
        return ctx.Caster.HasState(_blockedStateId)
            ? ConditionResult.Fail(SkillCastFailReason.CasterStateBlocked, _blockedStateId)
            : ConditionResult.Pass();
    }
}