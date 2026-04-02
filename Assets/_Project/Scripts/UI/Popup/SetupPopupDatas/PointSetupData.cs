[System.Serializable]
public class PointSetupData : BaseSetupData
{
    public int pointValue;

    public PointSetupData(string title, string validCharacters, int characterLimit, int pointValue) : base(title, validCharacters, characterLimit)
    {
        this.pointValue = pointValue;

    }
}