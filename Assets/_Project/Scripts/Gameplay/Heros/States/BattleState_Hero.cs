using System.Collections.Generic;
using UnityEngine;

public class BattleState_Hero : HeroState
{
    private int _nextSkillIndex = 0;
    private HeroBaseSkill heroSkills;
    public BattleState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
        heroSkills = hero.skillController;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        var list = heroSkills.GetAllSkills();
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            int idx = (_nextSkillIndex + i) % count;
            var skillDataRt = list[idx];

            if (heroSkills.GetSkill(skillDataRt.skillId)
                .IsReady(heroSkills.SkillController.Time.Now))
            {
                _nextSkillIndex = (idx + 1) % count;

                m_hero.currentSkillData = skillDataRt;
                m_machine.ChangeState(skillDataRt.skillAnimationClass);
                return;
            }
        }
        if(m_stateTimer <= 0)
        {
            m_stateTimer = m_hero.GetStats().AttackSpeed;
            m_machine.ChangeState<AttackState_Hero>();
        }
        else
            m_machine.ChangeState<IdleState_Hero>();
    }

    public override void Exit()
    {
        base.Exit();
    }
}