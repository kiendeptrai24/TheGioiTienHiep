using System.Collections.Generic;
using UnityEngine;

public class BattleState_Champion : ChampionState
{
    private int _nextSkillIndex = 0;
    private ChampionBaseSkill heroSkills;
    public BattleState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
        heroSkills = champion.skillController;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
    }

    public override void Exit()
    {
        base.Exit();
    }
}