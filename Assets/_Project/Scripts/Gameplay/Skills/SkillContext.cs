public sealed class SkillContext
{
    public readonly ITimeProvider Time;
    public readonly ISkillCaster Caster;
    public readonly ISkillTarget Target; // có thể null nếu skill self/no-target
    public readonly SkillRuntime Runtime; // runtime của skill đang xét

    public SkillContext(ITimeProvider time, ISkillCaster caster, ISkillTarget target, SkillRuntime runtime)
    {
        Time = time;
        Caster = caster;
        Target = target;
        Runtime = runtime;
    }
}