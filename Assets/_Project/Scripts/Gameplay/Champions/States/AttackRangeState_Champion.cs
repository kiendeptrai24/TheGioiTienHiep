using Unity.Netcode;
using UnityEngine;

public class AttackRangeState_Champion : ChampionState, ISkillTrigger, IAnimationTrigger
{
    public AttackRangeState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        var bullet = ObjectPool.Instance.GetObject(m_champion.attackPrefab, m_champion.transform.position, m_champion.transform.rotation);
        var bulletBase = bullet.GetComponent<BulletPlayBackBase>();
        var target = m_champion.findTarget.Target;
        bulletBase.SetUpTarGet(m_champion, target, m_champion.GetStats());
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