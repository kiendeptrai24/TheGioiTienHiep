using UnityEngine;

public sealed class SkillContext
{
    public readonly ITimeProvider Time;
    public readonly ISkillCaster Caster;
    public readonly ISkillTarget Target; // có thể null nếu skill self/no-target
    public readonly SkillRuntime Runtime; // runtime của skill đang xét
    public readonly SpawnPoint TargetDirection;


    public SkillContext(ITimeProvider time, ISkillCaster caster, ISkillTarget target, SkillRuntime runtime, SpawnPoint targetDirection)
    {
        Time = time;
        Caster = caster;
        Target = target;
        Runtime = runtime;
        TargetDirection = targetDirection;
    }
}