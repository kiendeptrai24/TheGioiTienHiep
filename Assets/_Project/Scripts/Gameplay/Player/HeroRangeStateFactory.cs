using System;
using System.Collections.Generic;

public class HeroRangeStateFactory : HeroStateFactory
{
    public HeroRangeStateFactory(HeroController hero, IStateMachine machine) : base(hero, machine)
    {
    }
    public override Dictionary<Type, IState> CreateState()
    {
        base.CreateState();
        _statesDictionary.Add(typeof(AttackState_Hero), new AttackState_Hero(_hero, _machine, "Atk_Range"));
        return _statesDictionary;
    }
}