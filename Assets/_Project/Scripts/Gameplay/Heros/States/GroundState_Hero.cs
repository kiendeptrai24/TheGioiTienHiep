
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class GroundState_Hero : HeroState
{
    private StatsData stats;
    public GroundState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
        stats = hero.GetComponent<StatsData>();
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

            if (Vector3.Distance(m_hero.transform.position, m_hero.m_aiMovement.Target.position) < stats.AttackRange)
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