using System;
using System.Collections.Generic;

public class HeroStateFactory : IStateFactory
{
    protected HeroController _hero;
    protected IStateMachine _machine;
    protected Dictionary<Type, IState> _statesDictionary;
    public HeroStateFactory(HeroController hero, IStateMachine machine)
    {
        _hero = hero;
        _machine = machine;
    }

    public void AddState(Type stateType, IState state)
    {
        _statesDictionary[stateType] = state;
    }

    public virtual Dictionary<Type, IState> CreateState()
    {
        _statesDictionary = new Dictionary<Type, IState>
        {
            {typeof(IdleState_Hero), new IdleState_Hero(_hero, _machine, "Idle")},
            {typeof(BattleState_Hero), new BattleState_Hero(_hero, _machine, "Idle")},
            {typeof(ChaseState_Hero), new ChaseState_Hero(_hero, _machine, "Move")},
            {typeof(MoveState_Hero), new MoveState_Hero(_hero, _machine, "Move")},

            {typeof(DonTramState_Hero), new DonTramState_Hero(_hero, _machine, "DonTram")},
            {typeof(LienKichChiThuatState_Hero), new LienKichChiThuatState_Hero(_hero, _machine, "LienKichChiThuat")},
            {typeof(ToanLucNhatKichState_Hero), new ToanLucNhatKichState_Hero(_hero, _machine, "ToanLucNhatKich")},
            {typeof(LinhTramState_Hero), new LinhTramState_Hero(_hero, _machine, "LinhTram")},
            {typeof(NhamChuanState_Hero), new NhamChuanState_Hero(_hero, _machine, "NhamChuan")},
            {typeof(LinhTienState_Hero), new LinhTienState_Hero(_hero, _machine, "LinhTien")},
            {typeof(VanLinhTienState_Hero), new VanLinhTienState_Hero(_hero, _machine, "VanLinhTien")},
            {typeof(VuTienState_Hero), new VuTienState_Hero(_hero, _machine, "VuTien")}
        };
        return _statesDictionary;
    }
}