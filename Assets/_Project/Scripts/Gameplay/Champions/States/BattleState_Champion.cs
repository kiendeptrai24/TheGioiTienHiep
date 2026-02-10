using System.Collections.Generic;
using UnityEngine;

public class BattleState_Champion : ChampionState
{
    private int _nextSkillIndex = 0;
    private HeroBaseSkill heroSkills;
    public BattleState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
        heroSkills = champion.skillController;
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

                m_champion.currentSkillData = skillDataRt;
                m_machine.ChangeState(skillDataRt.skillAnimationClass);
                return;
            }
        }
        if (m_stateTimer <= 0)
        {
            m_stateTimer = m_champion.GetStats().AttackSpeed;
            m_machine.ChangeState<AttackState_Champion>();
        }
        else
            m_machine.ChangeState<IdleState_Champion>();
    }

    public override void Exit()
    {
        base.Exit();
    }
}