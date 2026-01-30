
using Unity.VisualScripting;
using UnityEngine;

public class IdleState_EntityWorld : GroundState_EntityWorld
{
    public IdleState_EntityWorld(EntityWorldController entity, IStateMachine stateMachine, string animName) : base(entity, stateMachine, animName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
    }

    public override void Exit()
    {
        base.Exit();
    }
}