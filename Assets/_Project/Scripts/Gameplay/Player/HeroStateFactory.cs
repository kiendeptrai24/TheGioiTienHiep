using System;
using System.Collections.Generic;

public class HeroStateFactory : IStateFactory
{
    private readonly HeroController _hero;
    private readonly IStateMachine _machine;
    private Dictionary<Type, IState> _statesDictionary;
    public HeroStateFactory(HeroController hero, IStateMachine machine)
    {
        _hero = hero;
        _machine = machine;
    }

    public void AddState(Type stateType, IState state)
    {
        _statesDictionary[stateType] = state;
    }

    public Dictionary<Type, IState> CreateState()
    {
        _statesDictionary = new Dictionary<Type, IState>
        {
            {typeof(IdleState_Hero), new IdleState_Hero(_hero, _machine, "Idle")},
            {typeof(BattleState_Hero), new BattleState_Hero(_hero, _machine, "Idle")},
            {typeof(ChaseState_Hero), new ChaseState_Hero(_hero, _machine, "Move")},
            {typeof(MoveState_Hero), new MoveState_Hero(_hero, _machine, "Move")},
            {typeof(DonTramState_Hero), new DonTramState_Hero(_hero, _machine, "Attack")},
            {typeof(LinhTienState_Hero), new LinhTienState_Hero(_hero, _machine, "Attack2")},
            {typeof(LienKichChiThuatState_Hero), new LienKichChiThuatState_Hero(_hero, _machine, "Heal")},
            {typeof(ToanLucNhatKichState_Hero), new ToanLucNhatKichState_Hero(_hero, _machine, "WideArm")},
            {typeof(NhamChuanState_Hero), new NhamChuanState_Hero(_hero, _machine, "MagicArm")},
        };
        return _statesDictionary;
    }
}