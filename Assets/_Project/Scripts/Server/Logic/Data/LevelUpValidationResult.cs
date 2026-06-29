public class LevelUpValidationResult
{
    public bool result;
    public string playerId;
    public string instanceId;
    public string messege;
    public long startTime;
    public long endTime;
    public int Level;
    public bool isCompleted;
    public int rewardPotentialPoint;
    public int rewardSkillPoint;
    public LevelUpConditionType conditionType;
    public LevelUpValidationResult() { }
    public LevelUpValidationResult(bool valid, string msg)
    {
        result = valid;
        messege = msg;
    }
}