[System.Serializable]
public class BattleHistorySetupData : IPopupData
{
    public BattleHistoryDataPopup data;
    public string ValidCharacters { get; set; } = "";
    public int CharacterLimit { get; set; } = 50;
    public BattleHistorySetupData(BattleHistoryDataPopup shopSetupData, string validCharacters, int characterLimit)
    {
        this.data = shopSetupData;
        ValidCharacters = validCharacters;
        CharacterLimit = characterLimit;
    }
    public BattleHistorySetupData(BattleHistoryDataPopup shopSetupData)
    {
        this.data = shopSetupData;
    }
}