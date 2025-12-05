
using System;
using System.Collections.Generic;

public interface IStateFactory
{
    Dictionary<Type,IState> CreateState();
}