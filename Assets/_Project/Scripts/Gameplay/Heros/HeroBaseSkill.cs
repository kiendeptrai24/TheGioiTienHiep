

using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SkillDataRuntime
{
    public string skillId;
    public Type skillAnimationClass;
    public BaseSkill skill;
}

public class HeroBaseSkill : TGTHMonoBehaviour
{
    // SkillController to manage the hero's skills
    private SkillController m_SkillController;
    private HeroLoadData m_HeroLoadData;
    private FindTarget m_FindTargetEnemy;
    public SkillController SkillController => m_SkillController;
    // List of SkillData to be assigned in the Inspector
    private List<SkillData> m_SkillsData;
    private List<SkillDataRuntime> m_SkillsDataRuntimes;
    // target enemy for skills
    private GameObject m_EnemyTarget;
    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        m_SkillController = new SkillController(GetComponent<ISkillCaster>(), new UnityTimeProvider());
        m_SkillsDataRuntimes = new();
        m_SkillsData = new();

        if (m_HeroLoadData != null)
        {
            m_HeroLoadData.OnHeroDataLoaded += LoadHeroData;
        }
    }

    private void LoadHeroData(HeroData data)
    {
        m_SkillsData.AddRange(data.skillDatas);
        SetupSkills();
    }

    public void SetupSkills()
    {
        foreach (var skillData in m_SkillsData)
        {
            if (skillData == null) continue;
            SetupSkillClass(skillData.skillType, skillData);
        }
    }
    public void SetupSkillClass(SkillType skillType, SkillData skillData)
    {
        switch (skillType)
        {
            case SkillType.DonTram:
                var skillruntime = new FireballSkill(skillData, skillData.skillEffectPrefab, skillData.itemId, skillData.skillName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime,
                    skillAnimationClass = typeof(DonTramState_Hero),
                });
                m_SkillController.AddSkill(skillruntime);
                break;
            case SkillType.LinhTien:
                var skillruntime2 = new FireballSkill(skillData, skillData.skillEffectPrefab, skillData.itemId, skillData.skillName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime2,
                    skillAnimationClass = typeof(LinhTienState_Hero),
                });
                m_SkillController.AddSkill(skillruntime2);
                break;
            case SkillType.LienKichChiThuat:
                var skillruntime3 = new FocusSkill(skillData, skillData.skillEffectPrefab, skillData.itemId, skillData.skillName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime3,
                    skillAnimationClass = typeof(LienKichChiThuatState_Hero),
                });
                m_SkillController.AddSkill(skillruntime3);
                break;
            case SkillType.ToanLucNhatKich:
                var skillruntime4 = new FireballSkill(skillData, skillData.skillEffectPrefab, skillData.itemId, skillData.skillName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime4,
                    skillAnimationClass = typeof(ToanLucNhatKichState_Hero),
                });
                m_SkillController.AddSkill(skillruntime4);
                break;
            case SkillType.NhamChuan:
                var skillruntime5 = new FocusSkill(skillData, skillData.skillEffectPrefab, skillData.itemId, skillData.skillName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime5,
                    skillAnimationClass = typeof(NhamChuanState_Hero),
                });
                m_SkillController.AddSkill(skillruntime5);
                break;
            default:
                break;
        }
    }
    public void ActiveSkill(string skillname, SpawnPoint targetDirection)
    {
        if (m_FindTargetEnemy == null) return;
        ISkillTarget target = m_FindTargetEnemy;
        var skill = m_SkillController.GetRuntime(skillname);
        if (skill != null)
        {
            if (skill.IsReady(Time.time))
            {
                m_SkillController.TryCast(skillname, target, targetDirection);
            }
            else
            {
                Debug.Log($"Skill {skill.Skill.DisplayName} is on cooldown.");
            }
        }
    }
    public List<SkillDataRuntime> GetAllSkills()
    {
        return m_SkillsDataRuntimes;
    }
    public SkillRuntime GetSkill(string skillId)
    {
        return m_SkillController.GetRuntime(skillId);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        m_FindTargetEnemy = GetComponent<FindTarget>();
        m_HeroLoadData = GetComponent<HeroLoadData>();
    }
}