using Unity.Netcode;
using UnityEngine;

public class AttackState_Hero : HeroState, ISkillTrigger, IAnimationTrigger
{
    public AttackState_Hero(HeroController hero, IStateMachine stateMachine, string anim) : base(hero, stateMachine, anim)
    {

    }

    public void ActiveSkill()
    {
        Vector3 pos = m_hero.transform.position + Vector3.up * 1f;
        Quaternion rot = m_hero.transform.rotation;
        var networkSlash = GameObject.Instantiate(m_hero.attackPrefab, pos, rot);
        networkSlash.Spawn();
        var bullet = networkSlash.GetComponent<BulletBase>();
        var target = m_hero.target.target;
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