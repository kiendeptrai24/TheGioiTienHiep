using UnityEngine;

public class VuTienState_Hero : HeroState, ISkillTrigger, IAnimationTrigger
{
    public VuTienState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        SpawnPoint direction = new SpawnPoint();
        direction.position = m_hero.transform.position;
        direction.rotation = m_hero.transform.rotation;

        // dùng forward của hero, KHÔNG phải Vector3.forward
        direction.position += direction.rotation * Vector3.forward * 10f;
        direction.position += Vector3.up * 1f;


        m_hero.skillController.ActiveSkill(
            m_hero.currentSkillData.skillId,
            direction
        );
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