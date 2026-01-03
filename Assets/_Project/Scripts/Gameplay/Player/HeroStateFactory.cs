using System;
using System.Collections.Generic;

public class HeroStateFactory : IStateFactory
{
    private readonly HeroController _hero;
    private readonly IStateMachine _machine;
    
    public HeroStateFactory(HeroController hero, IStateMachine machine)
    {
        _hero = hero;
        _machine = machine;
    }

    public Dictionary<Type, IState> CreateState()
    {
        var HeroDictionary = new Dictionary<Type, IState>
        {
            {typeof(IdleState_Hero), new IdleState_Hero(_hero, _machine, "Idle")},
            {typeof(MoveState_Hero), new MoveState_Hero(_hero, _machine, "Move")}
        };
        return HeroDictionary;
    }

}