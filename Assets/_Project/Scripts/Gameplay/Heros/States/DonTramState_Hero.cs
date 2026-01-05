using UnityEngine;

public class DonTramState_Hero : HeroState, ISkillTrigger, IAnimationTrigger
{
    public DonTramState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        SpawnPoint direction = new SpawnPoint();
        direction.position = m_hero.transform.position;
        direction.rotation = m_hero.transform.rotation;
        m_hero.skillController.ActiveSkill(m_hero.currentSkillData.skillId, direction);
    }

    public void ActiveTrigger()
    {
        m_machine.ChangeState<IdleState_Hero>();
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