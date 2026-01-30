using System;
using System.Collections.Generic;

public abstract class EntityWorldStateMachine : IStateMachine
{
    protected Dictionary<Type, IState> _statesDirtionary = new Dictionary<Type, IState>();
    protected IState _curState;
    protected IStateFactory _factory;
    public EntityWorldStateMachine(EntityWorldController entity)
    {

    }
    public virtual void CreateState() => _statesDirtionary = _factory.CreateState();

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