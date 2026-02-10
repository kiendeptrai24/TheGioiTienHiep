
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class GroundState_Champion : ChampionState
{
    private StatsData stats;
    public GroundState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
        stats = champion.GetComponent<StatsData>();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        m_machine.ChangeState<BattleState_Champion>();
    }

    public override void Exit()
    {
        base.Exit();
    }

}