using System.Collections.Generic;
using UnityEngine;

public class BattleState_Hero : HeroState
{
    private int _nextSkillIndex = 0;
    public BattleState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        var list = m_hero.skillController.GetAllSkills();
        int count = list.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            int idx = (_nextSkillIndex + i) % count;
            var skillDataRt = list[idx];

            if (m_hero.skillController.GetSkill(skillDataRt.skillId).IsReady(m_hero.skillController.SkillController.Time.Now))
            {
                _nextSkillIndex = (idx + 1) % count;

                m_hero.currentSkillData = skillDataRt;
                m_machine.ChangeState(skillDataRt.skillAnimationClass);
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}