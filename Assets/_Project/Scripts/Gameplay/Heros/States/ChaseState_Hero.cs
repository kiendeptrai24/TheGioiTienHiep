using UnityEngine;

public class ChaseState_Hero : HeroState
{
    public ChaseState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        if (Vector3.Distance(m_hero.transform.position, m_hero.m_aiMovement.Target.position) >= m_hero.heroData.attackRange)
        {
            m_hero.m_aiMovement.SetTarget(m_hero.m_aiMovement.Target);
        }
        else
        {
            m_machine.ChangeState<BattleState_Hero>();
        }


    }

    public override void Exit()
    {
        base.Exit();
    }
}