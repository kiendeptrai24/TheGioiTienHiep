using UnityEngine;

public class MoveState_Champion : GroundState_Champion
{
    public MoveState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        if (m_champion.m_aiMovement != null && m_champion.m_aiMovement.IsMoving() == false)
            m_machine.ChangeState<IdleState_Champion>();
    }

    public override void Exit()
    {
        base.Exit();
    }
}