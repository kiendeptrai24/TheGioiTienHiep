using UnityEngine;

public class ToanLucNhatKichState_Champion : ChampionState, ISkillTrigger, IAnimationTrigger
{
    public ToanLucNhatKichState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        SpawnPoint direction = new SpawnPoint();
        direction.position = m_champion.transform.position;
        direction.rotation = m_champion.transform.rotation;
        m_champion.skillController.PlayBackActiveSkill(m_champion.currentSkillId, direction);
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