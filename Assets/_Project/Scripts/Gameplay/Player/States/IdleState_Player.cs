using Unity.VisualScripting;
using UnityEngine;

public class IdleState_Player : GroundState_Player
{
    private int idleRandomVal = 0;
    private string idleValName = "IdleValue";
    public IdleState_Player(PlayerController player, IStateMachine stateMachine, string anim) : base(player, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        SetRandomIdleVal();
        base.Enter();
    }

    private void SetRandomIdleVal()
    {
        idleRandomVal = Random.Range(0, 5);
        m_anim.SetFloat(idleValName, idleRandomVal);
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