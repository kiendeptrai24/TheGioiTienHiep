using Unity.Netcode;
using UnityEngine;

public class AttackRangeState_Hero : HeroState, ISkillTrigger, IAnimationTrigger
{
    private SkillContext skillContext;
    public AttackRangeState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        var networkSlash = GameObject.Instantiate(m_hero.attackPrefab, m_hero.transform.position, m_hero.transform.rotation);
        networkSlash.Spawn();
        var bullet = networkSlash.GetComponent<BulletBase>();
        var target = m_hero.target.Target;
        bullet.SetUpTarGet(m_hero, target, m_hero.GetStats());
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