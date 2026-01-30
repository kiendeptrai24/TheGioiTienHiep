using System;
using System.Collections.Generic;

public abstract class EntityWorldStateFactory : IStateFactory
{
    protected EntityWorldController _entity;
    protected IStateMachine _machine;
    protected Dictionary<Type, IState> _statesDictionary;
    public EntityWorldStateFactory(EntityWorldController entity, IStateMachine machine)
    {
        _entity = entity;
        _machine = machine;
    }

    public void AddState(Type stateType, IState state)
    {
        _statesDictionary[stateType] = state;
    }

    public virtual Dictionary<Type, IState> CreateState()
    {
        _statesDictionary = new Dictionary<Type, IState>
        {
            {typeof(IdleState_EntityWorld), new IdleState_EntityWorld(_entity, _machine, "Idle")},
        };
        return _statesDictionary;
    }
}