using System.Collections.Generic;

public class CheckLevelUpValidationResult
{
    public string message;
    public float finalBreakthroughRate;
    public bool result;
    public List<LevelUpValidationResult> results = new();
}