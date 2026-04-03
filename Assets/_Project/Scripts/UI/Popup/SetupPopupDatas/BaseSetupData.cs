[System.Serializable]
public class BaseSetupData : IPopupData
{
    public string Title { get; set; }
    public string ValidCharacters { get; set; } = "";
    public int CharacterLimit { get; set; } = 50;
    public BaseSetupData(string title, string validCharacters, int characterLimit)
    {
        Title = title;
        ValidCharacters = validCharacters;
        CharacterLimit = characterLimit;
    }
    public BaseSetupData(string title)
    {
        Title = title;
    }
    public BaseSetupData() { }
}