
using System;
using System.Collections.Generic;
[Serializable]
public class PlayerResource
{
    public RealmType realmType;
    public int linhThach;
    public int linhThao;
    public int khoangThach;
    public int yeuDan;
    public int maHach;
    public string requiredItem;
    public List<ItemAmount> itemAmounts = new();
    public void AddResource(string data)
    {
        if (string.IsNullOrEmpty(data))
            return;
        requiredItem = data;

        string[] items = data.Split(',');

        foreach (string item in items)
        {
            string[] parts = item.Split(':');

            if (parts.Length != 2)
                continue;

            string itemId = parts[0];

            if (!int.TryParse(parts[1], out int amount))
                continue;

            var itemAmount = new ItemAmount(itemId, amount);
            itemAmounts.Add(itemAmount);
        }
    }
}