using System;
using System.Collections.Generic;

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
    public List<ItemAmount> itemAmounts = new();
    public LevelUpConditionData(string requiredItem = "")
    {
        this.requiredItem = requiredItem;
        itemAmounts = ItemAmount.ParseItems(requiredItem);
    }
    public int GetTrucCoDan()
    {
        foreach (ItemAmount item in itemAmounts)
        {
            if (item.itemId == "ID_DANDUOC_TRUCCODAN_00001")
            {
                return item.amount;
            }
        }
        return 0;
    }
}
