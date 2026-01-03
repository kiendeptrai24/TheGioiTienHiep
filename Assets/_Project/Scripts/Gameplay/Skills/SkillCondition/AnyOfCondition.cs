using System.Collections.Generic;

public sealed class AnyOfCondition : ISkillCondition
{
    private readonly List<ISkillCondition> _conditions = new();

    public AnyOfCondition Add(ISkillCondition c) 
    { 
        if (c != null) 
            _conditions.Add(c);
        return this;
    }

    public ConditionResult Evaluate(in SkillContext ctx)
    {
        ConditionResult lastFail = ConditionResult.Fail(SkillCastFailReason.Custom, "No conditions");
        for (int i = 0; i < _conditions.Count; i++)
        {
            var r = _conditions[i].Evaluate(ctx);
            if (r.Ok) return r;
            lastFail = r;
        }
        return lastFail;
    }
}