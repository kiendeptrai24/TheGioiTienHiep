using UnityEngine;

public class MoveState_Hero : GroundState_Hero
{
    public MoveState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    
    public override void Excute()
    {
        base.Excute();
        if (m_hero.m_aiMovement != null && m_hero.m_aiMovement.IsMoving() == false)
            m_machine.ChangeState<IdleState_Hero>();
    }

    public override void Exit()
    {
        base.Exit();
    }
}