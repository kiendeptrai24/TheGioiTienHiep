using UnityEngine;

public class DonTramState_Champion : ChampionState, ISkillTrigger, IAnimationTrigger
{
    public DonTramState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        SpawnPoint direction = new SpawnPoint();
        direction.position = m_champion.transform.position;
        direction.rotation = m_champion.transform.rotation;
        m_champion.skillController.ActiveSkill(m_champion.currentSkillData.skillId, direction);
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