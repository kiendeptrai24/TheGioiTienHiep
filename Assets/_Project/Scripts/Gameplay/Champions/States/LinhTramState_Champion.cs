using UnityEngine;

public class LinhTramState_Champion : ChampionState, ISkillTrigger, IAnimationTrigger
{
    public LinhTramState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        SpawnPoint direction = new SpawnPoint();
        direction.position = m_champion.transform.position;
        direction.rotation = m_champion.transform.rotation;

        direction.position += direction.rotation * Vector3.forward;
        direction.position += Vector3.up * 1f;

        m_champion.skillController.PlayBackActiveSkill(
            m_champion.currentSkillId,
            direction
        );
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