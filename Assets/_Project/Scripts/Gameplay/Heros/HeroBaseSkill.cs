

using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroBaseSkill : TGTHMonoBehaviour, ISaveable
{
    [Serializable]
    public class SkillDataRuntime
    {
        public string SkillId;
        public string SkillName;
        public BaseSkill skill;
    }
    [SerializeField] private SkillController _skillController;
    public SkillController SkillController => _skillController;
    [SerializeField] private List<SkillData> _skillsData = new();
    [SerializeField] private List<SkillDataRuntime> _skillsDataRuntimes;
    [SerializeField] private GameObject skillEffectPrefab;
    public Transform target;
    [SerializeField] private string skillname;
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
            var skill = new FireballSkill(skillData, skillEffectPrefab, skillData.itemId, skillData.itemName, skillData.cooldown);
            _skillsDataRuntimes.Add(new SkillDataRuntime() { SkillId = skill.SkillId, SkillName = skill.DisplayName, skill = skill });
            _skillController.AddSkill(skill);
        }
        // //Add skill
        // _skillController.AddSkill(new FireballSkill("skill_fireball", "Fireball"));
        // ISkillTarget target = new EnemyTarget(this.target);
        // // Cast
        // var result = _skillController.TryCast("skill_fireball", target);
        // if (!result.Ok)
        // {
        //     Debug.Log($"Cast fail: {result.Reason} ({result.Note})");
        // }

        // Remove skill
        //_skillController.RemoveSkill("skill_fireball");
    }
    [ContextMenu("ActiveSkill")]
    public void ActiveSkill()
    {
        ISkillTarget target = new EnemyTarget(this.target);
        var result = _skillController.TryCast(skillname, target);
        if (!result.Ok)
        {
            Debug.Log($"Cast fail: {result.Reason} ({result.Note})");
        }
        else
        {
            Debug.Log($"Cast success: {skillname}");
        }
    }
    public void LoadData(GameData _data)
    {
        foreach (var data in _data.itemDatas)
        {
            if(data is SkillData skillData)
            {
                _skillsData.Add(skillData);
            }
        }
        SetupSkills();
    }

    public void SaveGame(ref GameData _data)
    {
        
    }
}