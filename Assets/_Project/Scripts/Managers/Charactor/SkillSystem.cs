


using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : TGTHMonoBehaviour 
{
    List<SkillData> skills = new List<SkillData>();

    public void AddSkill(SkillData skill)
    {
        skills.Add(skill);
    }
}