using System;
using System.Collections.Generic;

public class EnemyWorldStateFactory : EntityWorldStateFactory
{
    public EnemyWorldStateFactory(EntityWorldController entity, IStateMachine machine) : base(entity, machine)
    {
    }
    public override Dictionary<Type, IState> CreateState()
    {
        return base.CreateState();
    }
}