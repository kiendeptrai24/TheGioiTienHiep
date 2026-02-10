using Unity.Netcode;
using UnityEngine;

public class AttackState_Champion : ChampionState, ISkillTrigger, IAnimationTrigger
{
    public AttackState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {

    }

    public void ActiveSkill()
    {
        Vector3 pos = m_champion.transform.position + Vector3.up * 1f;
        Quaternion rot = m_champion.transform.rotation;
        var networkSlash = GameObject.Instantiate(m_champion.attackPrefab, pos, rot);
        var bullet = networkSlash.GetComponent<BulletBase>();
        var target = m_champion.Target;
        bullet.SetUpTarGet(m_champion, target, m_champion.GetStats());
    }

    public void ActiveTrigger()
    {
        m_machine.ChangeState<IdleState_Champion>();
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