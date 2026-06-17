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
        var slash = ObjectPool.Instance.GetObject(m_champion.attackPrefab, pos, rot);
        ObjectPool.Instance.ReturnObject(slash, 1);
        var clip = PlayerSoundManager.Instance.GetSound("melee-attack");
        m_champion.PlayAudio(clip);
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