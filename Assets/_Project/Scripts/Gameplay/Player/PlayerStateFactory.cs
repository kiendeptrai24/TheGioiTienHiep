using System;
using System.Collections.Generic;

public class PlayerStateFactory : IStateFactory
{
    private readonly PlayerController _player;
    private readonly IStateMachine _machine;
    private Dictionary<Type, IState> _statesDictionary;
    public PlayerStateFactory(PlayerController player, IStateMachine machine)
    {
        _player = player;
        _machine = machine;
    }
    public void AddState(Type stateType, IState state)
    {
        _statesDictionary[stateType] = state;
    }
    public Dictionary<Type, IState> CreateState()
    {
        _statesDictionary = new Dictionary<Type, IState>
        {
            {typeof(IdleState_Player), new IdleState_Player(_player, _machine, "Idle")},
            {typeof(MoveState_Player), new MoveState_Player(_player, _machine, "Move")}
        };
        return _statesDictionary;
    }

}