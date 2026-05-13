

using System;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class ChampionAnimationPlayback : TGTHMonoBehaviour, IChampionAnimation
{
    public ChampionController champion;
    private IStateMachine m_machine;
    private ChampionBaseSkill chamSkills;
    private AIChampionMovement aiMovement;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    override protected void Start()
    {
        base.Start();
        m_machine = champion.GetStateMachine();
    }

    public void PlayAnimationAttack()
    {
        if (champion == null)
            m_machine = champion.GetStateMachine();
        if (champion.isMeleeChampion)
            m_machine.ChangeState<AttackState_Champion>();
        else
            m_machine.ChangeState<AttackRangeState_Champion>();
    }

    public void PlayMovement(Vector3 destination)
    {
        if (aiMovement != null)
        {
            aiMovement.SetDetinition(destination);
        }
    }

    public void PlayAnimationSkill(string skillid)
    {
        if (chamSkills.HasSkill(skillid))
        {
            var skillData = chamSkills.GetSkill(skillid);
            if (skillData.Skill == null)
            {
                return;
            }
            champion.currentSkillId = skillid;
            m_machine.ChangeState(skillData.Skill.skillAnimationClass);
        }
    }
    override protected void LoadComponent()
    {
        base.LoadComponent();
        champion = GetComponent<ChampionController>();
        chamSkills = GetComponent<ChampionBaseSkill>();
        aiMovement = GetComponent<AIChampionMovement>();
    }
}