

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
    private FindTarget m_FindTargetEnemy;
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
        switch (skillType)
        {
            case SkillType.DonTram:
                var skillruntime = new NetworkIdentifySkill(skillData, skillData.networkSkillEffectPrefab,
                skillData.itemId, skillData.itemName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime,
                    skillAnimationClass = typeof(DonTramState_Hero),
                });
                m_SkillController.AddSkill(skillruntime);
                break;
            case SkillType.LinhTien:
                var skillruntime2 = new NetworkFocusSkill(skillData, skillData.networkSkillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime2,
                    skillAnimationClass = typeof(LinhTienState_Hero),
                });
                m_SkillController.AddSkill(skillruntime2);
                break;
            case SkillType.LienKichChiThuat:
                var skillruntime3 = new NetworkIdentifySkill(skillData, skillData.networkSkillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime3,
                    skillAnimationClass = typeof(LienKichChiThuatState_Hero),
                });
                m_SkillController.AddSkill(skillruntime3);
                break;
            case SkillType.ToanLucNhatKich:
                var skillruntime4 = new NetworkIdentifySkill(skillData, skillData.networkSkillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime4,
                    skillAnimationClass = typeof(ToanLucNhatKichState_Hero),
                });
                m_SkillController.AddSkill(skillruntime4);
                break;
            case SkillType.NhamChuan:
                var skillruntime5 = new NetworkFocusSkill(skillData, skillData.networkSkillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime5,
                    skillAnimationClass = typeof(NhamChuanState_Hero),
                });
                m_SkillController.AddSkill(skillruntime5);
                break;
            case SkillType.VanLinhTien:
                var skillruntime6 = new NetworkFocusSkill(skillData, skillData.networkSkillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime6,
                    skillAnimationClass = typeof(VanLinhTienState_Hero)
                });
                m_SkillController.AddSkill(skillruntime6);
                break;
            case SkillType.LinhTram:
                var skillruntime7 = new NetworkIdentifySkill(skillData, skillData.networkSkillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime7,
                    skillAnimationClass = typeof(LinhTramState_Hero)
                });
                m_SkillController.AddSkill(skillruntime7);
                break;
            case SkillType.VuTien:
                var skillruntime8 = new NetworkFocusSkill(skillData, skillData.networkSkillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown);

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime8,
                    skillAnimationClass = typeof(VuTienState_Hero)
                });
                m_SkillController.AddSkill(skillruntime8);
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
        m_FindTargetEnemy = GetComponent<FindTarget>();
        statsData = GetComponent<StatsData>();
    }
}