using System;
using UnityEngine;
[Serializable]
public sealed class SkillRuntime
{
    [SerializeField] private BaseSkill _skill;

    public BaseSkill Skill => _skill;
    public float NextReadyTime { get; private set; } = 0f;

    public SkillRuntime(BaseSkill skill)
    {
        _skill = skill;
    }

    public bool IsReady(float now) => now >= NextReadyTime;

    public void TriggerCooldown(float now, float cd)
    {
        // Nếu muốn “cooldown cộng dồn”, đổi logic ở đây
        NextReadyTime = Math.Max(NextReadyTime, now) + Math.Max(0f, cd);
    }

    public float CooldownRemaining(float now) => Math.Max(0f, NextReadyTime - now);
}