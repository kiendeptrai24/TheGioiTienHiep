using UnityEngine;

public class MoveState_Player : GroundState_Player
{
    public MoveState_Player(PlayerController player, IStateMachine stateMachine, string anim) : base(player, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
        PlayerSoundManager.Instance.PlayClip("move", true);
    }

    public override void Excute()
    {
        base.Excute();
        if (m_player.moveable != null && m_player.moveable.IsMoving() == false)
            m_machine.ChangeState<IdleState_Player>();
    }

    public override void Exit()
    {
        base.Exit();
        PlayerSoundManager.Instance.StopMusic();
    }
}