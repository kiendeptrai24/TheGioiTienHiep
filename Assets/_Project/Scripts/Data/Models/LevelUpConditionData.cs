using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelUpValidator;

[Serializable]
public class LevelUpConditionData
{
    public int level;
    public LevelUpConditionType conditionType;           
    public string levelName;
    public int linhThao;
    public int khoangThach;
    public int yeuDan;
    public int maHach;
    public int linhThach;
    public string requiredItem;
    public int requiredCharacterLevel;
}
