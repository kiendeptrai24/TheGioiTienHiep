using System;
using System.Collections.Generic;

[Serializable]
public class LevelUpConditionData
{
    private const string TrucCoDanBaseId = "ID_DANDUOC_TRUCCODAN";
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

    public List<ItemAmount> GetBreakthroughPills()
    {
        List<ItemAmount> result = new();
        foreach (ItemAmount item in itemAmounts)
        {
            if (item.itemId == TrucCoDanBaseId)
                result.Add(item);
        }

        return result;
    }

    public int GetTrucCoDan()
    {
        int amount = 0;
        foreach (ItemAmount item in GetBreakthroughPills())
        {
            amount += item.amount;
        }

        return amount;
    }
}
