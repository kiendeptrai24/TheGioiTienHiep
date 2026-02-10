using Unity.Netcode;
using UnityEngine;

public class AttackRangeState_Champion : ChampionState, ISkillTrigger, IAnimationTrigger
{
    private SkillContext skillContext;
    public AttackRangeState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        var networkSlash = GameObject.Instantiate(m_champion.attackPrefab, m_champion.transform.position, m_champion.transform.rotation);
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