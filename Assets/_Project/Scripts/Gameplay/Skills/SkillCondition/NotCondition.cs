public sealed class NotCondition : ISkillCondition
{
    private readonly ISkillCondition _inner;
    private readonly SkillCastFailReason _reasonWhenInnerPass;

    public NotCondition(ISkillCondition inner, SkillCastFailReason reasonWhenInnerPass = SkillCastFailReason.Custom)
    {
        _inner = inner;
        _reasonWhenInnerPass = reasonWhenInnerPass;
    }

    public ConditionResult Evaluate(in SkillContext ctx)
    {
        var r = _inner.Evaluate(ctx);
        return r.Ok ? ConditionResult.Fail(_reasonWhenInnerPass) : ConditionResult.Pass();
    }
}