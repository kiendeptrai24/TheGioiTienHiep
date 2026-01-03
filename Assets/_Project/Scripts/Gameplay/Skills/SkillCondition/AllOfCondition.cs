using System.Collections.Generic;

public sealed class AllOfCondition : ISkillCondition
{
    private readonly List<ISkillCondition> _conditions = new();

    public AllOfCondition Add(ISkillCondition c) 
    { 
        if (c != null) 
            _conditions.Add(c); 
        return this; 
    }

    public ConditionResult Evaluate(in SkillContext ctx)
    {
        for (int i = 0; i < _conditions.Count; i++)
        {
            var r = _conditions[i].Evaluate(ctx);
            if (!r.Ok) return r;
        }
        return ConditionResult.Pass();
    }
}