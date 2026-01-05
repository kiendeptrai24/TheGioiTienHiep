

using System;

public interface IStateMachine
{
    void CreateState();
    IState GetCurrentState();
    void Init<T>()  where T : IState;
    void ChangeState<T>() where T : IState;
    void ChangeState(Type stateType);
    void SetState(IState curState);
    IState GetState<T>()  where T : IState;
    T GetFeature<T>() where T : class;
    void Update();
}