using System;
using System.Collections.Generic;

public class PlayerStateFactory : IStateFactory
{
    private readonly PlayerController _player;
    private readonly IStateMachine _machine;
    public PlayerStateFactory(PlayerController player, IStateMachine machine)
    {
        _player = player;
        _machine = machine;
    }

    public Dictionary<Type, IState> CreateState()
    {
        var Playerdictionary = new Dictionary<Type, IState>
        {
            {typeof(IdleState_Player), new IdleState_Player(_player, _machine, "Idle")},
            {typeof(MoveState_Player), new MoveState_Player(_player, _machine, "Move")}
        };
        return Playerdictionary;
    }

}