using System;
using System.Collections.Generic;

public class HeroRangeStateMachine : HeroStateMachine
{
    public HeroRangeStateMachine(HeroController hero) : base(hero)
    {
        _factory = new HeroRangeStateFactory(hero, this);
        CreateState();
    }
}