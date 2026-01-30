using System;
using System.Collections.Generic;

public class EnemyWorldStateMachine : EntityWorldStateMachine
{
    public EnemyWorldStateMachine(EntityWorldController entity) : base(entity)
    {
        _factory = new EnemyWorldStateFactory(entity, this);
        CreateState();
    }
}