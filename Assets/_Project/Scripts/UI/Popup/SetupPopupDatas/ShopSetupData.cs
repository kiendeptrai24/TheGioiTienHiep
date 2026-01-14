[System.Serializable]
public class ShopSetupData : IPopupData
{
    public ShopDataPopup data;
    public string ValidCharacters { get; set; } = "";
    public int CharacterLimit { get; set; } = 50;
    public ShopSetupData(ShopDataPopup shopSetupData, string validCharacters, int characterLimit)
    {
        this.data = shopSetupData;
        ValidCharacters = validCharacters;
        CharacterLimit = characterLimit;
    }
    public ShopSetupData(ShopDataPopup shopSetupData)
    {
        this.data = shopSetupData;
    }
}