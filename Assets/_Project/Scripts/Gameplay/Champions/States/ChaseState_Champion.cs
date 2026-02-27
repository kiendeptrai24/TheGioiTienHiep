using UnityEngine;

public class ChaseState_Champion : ChampionState
{
    public ChaseState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        m_champion.m_aiMovement.SetDetinition(m_champion.m_aiMovement.Target);
    }

    public override void Exit()
    {
        base.Exit();
    }
}