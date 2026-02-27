using System;
using System.Collections.Generic;

public class ChampionStateFactory : IStateFactory
{
    private readonly ChampionController _Champion;
    private readonly IStateMachine _machine;
    private Dictionary<Type, IState> _statesDictionary;
    public ChampionStateFactory(ChampionController champion, IStateMachine machine)
    {
        _Champion = champion;
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
            {typeof(IdleState_Champion), new IdleState_Champion(_Champion, _machine, "Idle")},
            {typeof(MoveState_Champion), new MoveState_Champion(_Champion, _machine, "Move")},
            {typeof(BattleState_Champion), new BattleState_Champion(_Champion, _machine, "Idle")},
            {typeof(ChaseState_Champion), new ChaseState_Champion(_Champion, _machine, "Move")},

            {typeof(AttackState_Champion), new AttackState_Champion(_Champion, _machine, "Atk_Melee")},
            {typeof(AttackRangeState_Champion), new AttackRangeState_Champion(_Champion, _machine, "Atk_Range")},


            {typeof(DonTramState_Champion), new DonTramState_Champion(_Champion, _machine, "DonTram")},
            {typeof(LienKichChiThuatState_Champion), new LienKichChiThuatState_Champion(_Champion, _machine, "LienKichChiThuat")},
            {typeof(ToanLucNhatKichState_Champion), new ToanLucNhatKichState_Champion(_Champion, _machine, "ToanLucNhatKich")},
            {typeof(LinhTramState_Champion), new LinhTramState_Champion(_Champion, _machine, "LinhTram")},
            {typeof(NhamChuanState_Champion), new NhamChuanState_Champion(_Champion, _machine, "NhamChuan")},
            {typeof(LinhTienState_Champion), new LinhTienState_Champion(_Champion, _machine, "LinhTien")},
            {typeof(VanLinhTienState_Champion), new VanLinhTienState_Champion(_Champion, _machine, "VanLinhTien")},
            {typeof(VuTienState_Champion), new VuTienState_Champion(_Champion, _machine, "VuTien")}
        };
        return _statesDictionary;
    }

}