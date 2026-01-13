using System;
using System.Collections.Generic;

public class HeroMeleeStateMachine : HeroStateMachine
{
    public HeroMeleeStateMachine(HeroController hero) : base(hero)
    {
        _factory = new HeroMeleeStateFactory(hero, this);
        CreateState();
    }
}