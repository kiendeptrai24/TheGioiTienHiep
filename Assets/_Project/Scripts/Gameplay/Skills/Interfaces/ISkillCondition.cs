public interface ISkillCondition
{
    ConditionResult Evaluate(in SkillContext ctx);
}