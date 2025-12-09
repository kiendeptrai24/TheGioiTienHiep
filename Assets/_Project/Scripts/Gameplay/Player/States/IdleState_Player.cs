using Unity.VisualScripting;
using UnityEngine;

public class IdleState_Player : GroundState_Player
{

    public IdleState_Player(PlayerController player, IStateMachine stateMachine, string anim) : base(player, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        if (m_player.moveable != null && m_player.moveable.IsMoving() == true)
            m_machine.ChangeState<MoveState_Player>();
    }
    
    public override void Exit()
    {
        base.Exit();
    }
}