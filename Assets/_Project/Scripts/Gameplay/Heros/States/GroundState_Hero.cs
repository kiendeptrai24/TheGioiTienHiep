
using UnityEngine;

public class GroundState_Hero : HeroState
{
    public GroundState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();

        if (m_hero.m_aiMovement.Target != null && m_hero.heroData != null)
        {
            if (Vector3.Distance(m_hero.transform.position, m_hero.m_aiMovement.Target.position) < m_hero.heroData.attackRange)
            {
                m_machine.ChangeState<BattleState_Hero>();
                return;
            }
            m_machine.ChangeState<ChaseState_Hero>();
        }


    }

    public override void Exit()
    {
        base.Exit();
    }

}