

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
    private StatsData statsData;
    private TargetFinderBase m_FindTargetEnemy;
    public SkillController SkillController => m_SkillController;
    // List of SkillData to be assigned in the Inspector
    private List<SkillData> m_SkillsData;
    private List<SkillDataRuntime> m_SkillsDataRuntimes;
    public UnityTimeProvider timeProvider;
    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        timeProvider = new UnityTimeProvider();
        m_SkillController = new SkillController(GetComponent<ISkillCaster>(), timeProvider);
        m_SkillsDataRuntimes = new();
        m_SkillsData = new();

        if (statsData != null)
        {
            var data = statsData.heroPreset.GetItemData();
            var heroData = data as HeroData;
            m_SkillsData.AddRange(heroData.skillDatas);
            SetupSkills();
        }
        Debug.Log($"HeroBaseSkill Awake: Loaded {m_SkillsData.Count} skills for hero {gameObject.name}");
    }

    public void SetupSkills()
    {
        foreach (var skillData in m_SkillsData)
        {
            if (skillData == null) continue;
            SetupSkillHeroClass(skillData.skillType, skillData);

        }
    }
    public void SetupSkillHeroClass(SkillType skillType, SkillData skillData)
    {

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
    public bool HasSkill(string skillId)
    {
        return m_SkillController.HasSkill(skillId);
    }
    public SkillRuntime GetSkill(string skillId)
    {
        return m_SkillController.GetRuntime(skillId);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        m_FindTargetEnemy = GetComponent<TargetFinderBase>();
        statsData = GetComponent<StatsData>();
    }
}