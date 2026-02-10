using UnityEngine;

public class IdleState_Champion : GroundState_Champion
{

    public IdleState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        if (m_champion.m_aiMovement != null && m_champion.m_aiMovement.IsMoving() == true)
            m_machine.ChangeState<MoveState_Champion>();
    }

    public override void Exit()
    {
        base.Exit();
    }
}