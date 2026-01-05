
using UnityEngine;

public class GroundState_Hero : HeroState
{
    public GroundState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Excute()
    {
        base.Excute();
        foreach (var skillRuntime in m_hero.skillController.GetAllSkillRuntimes())
        {
            if(m_hero.skillController.GetSkill(skillRuntime.skillId).IsReady(Time.time))
            {
                m_hero.currentSkillData = skillRuntime;
                m_machine.ChangeState(skillRuntime.skillAnimationClass);
                break;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

}