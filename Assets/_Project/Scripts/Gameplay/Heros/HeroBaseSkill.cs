

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

public class HeroBaseSkill : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private string heroName;
    [SerializeField] private HeroData _heroData;
    [SerializeField] private SkillController _skillController;
    public SkillController SkillController => _skillController;
    [SerializeField] private List<SkillData> _skillsData = new();
    [SerializeField] private List<SkillDataRuntime> _skillsDataRuntimes;
    public Transform target;
    override protected void Awake()
    {
        base.Awake();
        _skillController = new SkillController(GetComponent<ISkillCaster>(), new UnityTimeProvider());
    }
    public void SetupSkills()
    {
        foreach (var skillData in _skillsData)
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

                _skillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime,
                    skillAnimationClass = typeof(DonTramState_Hero),
                });
                _skillController.AddSkill(skillruntime);
                break;
            case SkillType.LinhTien:
                var skillruntime2 = new FireballSkill(skillData, skillData.skillEffectPrefab, skillData.itemId, skillData.skillName, skillData.cooldown);

                _skillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime2,
                    skillAnimationClass = typeof(LinhTienState_Hero),
                });
                _skillController.AddSkill(skillruntime2);
                break;
            case SkillType.LienKichChiThuat:
                var skillruntime3 = new FireballSkill(skillData, skillData.skillEffectPrefab, skillData.itemId, skillData.skillName, skillData.cooldown);

                _skillsDataRuntimes.Add(new SkillDataRuntime()
                {
                    skillId = skillData.itemId,
                    skill = skillruntime3,
                    skillAnimationClass = typeof(LienKichChiThuatState_Hero),
                });
                _skillController.AddSkill(skillruntime3);
                break;
            default:
                break;
        }
    }
    public Type GetSkillAnimationClass(string skillId)
    {
        foreach (var skillRuntime in _skillsDataRuntimes)
        {
            if (skillRuntime.skillId == skillId)
            {
                return skillRuntime.skillAnimationClass;
            }
        }
        return null;
    }
    public void ActiveSkill(string skillname, SpawnPoint targetDirection)
    {
        ISkillTarget target = new EnemyTarget(this.target);
        var skill = _skillController.GetRuntime(skillname);
        if (skill != null)
        {
            if (skill.IsReady(_skillController.Time.Now))
            {
                _skillController.TryCast(skillname, target, targetDirection);
            }
            else
            {
                Debug.Log($"Skill {skill.Skill.DisplayName} is on cooldown.");
            }
        }
    }
    public List<SkillDataRuntime> GetAllSkillRuntimes()
    {
        return _skillsDataRuntimes;
    }
    public SkillRuntime GetSkill(string skillId)
    {
        return _skillController.GetRuntime(skillId);
    }
    public void LoadData(GameData _data)
    {
        foreach (var data in _data.itemDatas)
        {
            if (data is HeroData heroData)
            {
                if(heroData.itemName == heroName)
                {
                    _heroData = heroData;
                    break;
                }
            }
        }
        _skillsData.AddRange(_heroData.skillDatas);
        Debug.Log(_skillsData.Count);
        SetupSkills();
    }

    public void SaveGame(ref GameData _data)
    {

    }
}