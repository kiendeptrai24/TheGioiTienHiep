public sealed class ManaCondition : ISkillCondition
{
    private readonly float _manaCost;
    public ManaCondition(float manaCost) => _manaCost = manaCost;

    public ConditionResult Evaluate(in SkillContext ctx)
    {
        return ctx.Caster.Mana >= _manaCost
            ? ConditionResult.Pass()
            : ConditionResult.Fail(SkillCastFailReason.NotEnoughMana);
    }
}