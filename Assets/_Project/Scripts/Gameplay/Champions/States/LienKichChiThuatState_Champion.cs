using UnityEngine;

public class LienKichChiThuatState_Champion : ChampionState, ISkillTrigger, IAnimationTrigger
{
    public LienKichChiThuatState_Champion(ChampionController champion, IStateMachine stateMachine, string anim) : base(champion, stateMachine, anim)
    {
    }

    public void ActiveSkill()
    {
        SpawnPoint direction = new SpawnPoint();
        direction.position = m_champion.transform.position;
        direction.rotation = m_champion.transform.rotation;

        // dùng forward của hero, KHÔNG phải Vector3.forward
        direction.position += direction.rotation * Vector3.forward * 10f;
        direction.position += Vector3.up * 1f;


        m_champion.skillController.ActiveSkill(
            m_champion.currentSkillData.skillId,
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