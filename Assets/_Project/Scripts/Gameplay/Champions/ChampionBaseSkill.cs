
using System.Collections.Generic;
using UnityEngine;

public class ChampionBaseSkill : TGTHMonoBehaviour
{
    // SkillController to manage the hero's skills
    private SkillController m_SkillController;
    private StatsData m_StatsData;
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
        SetupSkills();
    }

    public void SetupSkills()
    {
        var data = (m_StatsData.heroData as HeroData).skillDatas;
        m_SkillsData = data;
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
                var skillruntime = new IdentifySkill(skillData, skillData.skillEffectPrefab,
                skillData.itemId, skillData.itemName, skillData.cooldown, typeof(DonTramState_Champion));

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime,
                    skillAnimationClass = typeof(DonTramState_Champion),
                });
                m_SkillController.AddSkill(skillruntime);
                break;
            case SkillType.LinhTien:
                var skillruntime2 = new FocusSkill(skillData, skillData.skillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown, typeof(LinhTienState_Champion));

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime2,
                    skillAnimationClass = typeof(LinhTienState_Champion),
                });
                m_SkillController.AddSkill(skillruntime2);
                break;
            case SkillType.LienKichChiThuat:
                var skillruntime3 = new IdentifySkill(skillData, skillData.skillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown, typeof(LienKichChiThuatState_Champion));

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime3,
                    skillAnimationClass = typeof(LienKichChiThuatState_Champion),
                });
                m_SkillController.AddSkill(skillruntime3);
                break;
            case SkillType.ToanLucNhatKich:
                var skillruntime4 = new IdentifySkill(skillData, skillData.skillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown, typeof(ToanLucNhatKichState_Champion));

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime4,
                    skillAnimationClass = typeof(ToanLucNhatKichState_Champion),
                });
                m_SkillController.AddSkill(skillruntime4);
                break;
            case SkillType.NhamChuan:
                var skillruntime5 = new FocusSkill(skillData, skillData.skillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown, typeof(NhamChuanState_Champion));

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime5,
                    skillAnimationClass = typeof(NhamChuanState_Champion),
                });
                m_SkillController.AddSkill(skillruntime5);
                break;
            case SkillType.VanLinhTien:
                var skillruntime6 = new FocusSkill(skillData, skillData.skillEffectPrefab,
                skillData.itemId, skillData.itemName, skillData.cooldown, typeof(VanLinhTienState_Champion));

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime6,
                    skillAnimationClass = typeof(VanLinhTienState_Champion)
                });
                m_SkillController.AddSkill(skillruntime6);
                break;
            case SkillType.LinhTram:
                var skillruntime7 = new IdentifySkill(skillData, skillData.skillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown, typeof(LinhTramState_Champion));

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime7,
                    skillAnimationClass = typeof(LinhTramState_Champion)
                });
                m_SkillController.AddSkill(skillruntime7);
                break;
            case SkillType.VuTien:
                var skillruntime8 = new FocusSkill(skillData, skillData.skillEffectPrefab,
                 skillData.itemId, skillData.itemName, skillData.cooldown, typeof(VuTienState_Champion));

                m_SkillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime8,
                    skillAnimationClass = typeof(VuTienState_Champion)
                });
                m_SkillController.AddSkill(skillruntime8);
                break;
            default:
                break;
        }
    }
    public void PlayBackActiveSkill(string skillname, SpawnPoint targetDirection)
    {
        if (m_FindTargetEnemy == null) return;
        ISkillTarget target = m_FindTargetEnemy;
        var skill = m_SkillController.GetRuntime(skillname);
        if (skill != null)
        {
            var result = m_SkillController.TryCast(skillname, target, targetDirection);
            if (!result.Ok)
            {
                Debug.Log("Reason " + result.Reason);
            }
        }
        else
        {
            Debug.Log("Skill not found");
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
        m_StatsData = GetComponent<StatsData>();
    }
}