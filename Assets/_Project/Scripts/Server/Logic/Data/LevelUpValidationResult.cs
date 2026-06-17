public class LevelUpValidationResult
{
    public bool result;
    public string playerId;
    public string instanceId;
    public string message;
    public long startTime;
    public long endTime;
    public int Level;
    public bool isCompleted;
    public LevelUpConditionType conditionType;
    public LevelUpValidationResult() { }
    public LevelUpValidationResult(bool valid, string msg)
    {
        result = valid;
        message = msg;
    }
}