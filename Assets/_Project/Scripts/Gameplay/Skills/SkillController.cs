using System;
using System.Collections.Generic;
[Serializable]
public sealed class SkillController
{

    private readonly ISkillCaster _owner;
    private readonly ITimeProvider _time;

    private readonly Dictionary<string, SkillRuntime> _skills = new();
    public SkillController(ISkillCaster owner, ITimeProvider timeProvider)
    {
        _owner = owner;
        _time = timeProvider;
    }

    public bool HasSkill(string skillId) => _skills.ContainsKey(skillId);

    public SkillRuntime GetRuntime(string skillId)
        => _skills.TryGetValue(skillId, out var rt) ? rt : null;

    public void AddSkill(BaseSkill skill)
    {
        if (skill == null) return;
        if (_skills.ContainsKey(skill.SkillId)) return;

        skill.BuildDefaultConditions();
        _skills.Add(skill.SkillId, new SkillRuntime(skill));
    }

    public bool RemoveSkill(string skillId) => _skills.Remove(skillId);

    public SkillCastResult TryCast(string skillId, ISkillTarget target)
    {
        if (!_skills.TryGetValue(skillId, out var rt))
            return SkillCastResult.Fail(SkillCastFailReason.Custom, "Skill not found");

        var ctx = new SkillContext(_time, _owner, target, rt);
        var can = rt.Skill.CanCast(ctx);
        if (!can.Ok) return SkillCastResult.Fail(can.Reason, can.DebugNote);

        rt.Skill.OnCastSucceeded(ctx);
        return SkillCastResult.Success();
    }
}