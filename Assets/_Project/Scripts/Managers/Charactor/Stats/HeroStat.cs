
using System.Collections;
using UnityEngine;

public class HeroStat : CharacterStats 
{
    public override void TakeDamage(SkillContext ctx, StatsData _targetStats)
    {
        base.TakeDamage(ctx, _targetStats);
    }
}

