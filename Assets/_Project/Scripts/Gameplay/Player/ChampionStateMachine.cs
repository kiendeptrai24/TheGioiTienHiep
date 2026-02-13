using System;
using System.Collections.Generic;
using UnityEngine;

public class ChampionStateMachine : IStateMachine
{
    private Dictionary<Type, IState> _statesDirtionary = new Dictionary<Type, IState>();
    private IState _curState;
    private readonly IStateFactory _factory;

    public ChampionStateMachine(ChampionController champion)
    {
        _factory = new ChampionStateFactory(champion, this);
        CreateState();
    }

    public void CreateState() => _statesDirtionary = _factory.CreateState();

    public void Init<T>() where T : IState
    {
        if (GetState<T>() == null)
            return;
        SetState(GetState<T>());

        _curState.Enter();
    }

    public void ChangeState<T>() where T : IState
    {
        if (GetState<T>() == null)
            return;
        _curState.Exit();
        Init<T>();
    }

    public void SetState(IState curState) => _curState = curState;

    public IState GetState<T>() where T : IState => _statesDirtionary[typeof(T)];

    public void Update()
    {
        if (_curState != null)
            _curState.Excute();
    }

    public IState GetCurrentState() => _curState;

    public T GetFeature<T>() where T : class
    {
        return _curState as T;
    }
    public void ChangeState(Type stateType)
    {
        if (!_statesDirtionary.TryGetValue(stateType, out var nextState))
            return;

        _curState?.Exit();
        _curState = nextState;
        _curState.Enter();
    }
}