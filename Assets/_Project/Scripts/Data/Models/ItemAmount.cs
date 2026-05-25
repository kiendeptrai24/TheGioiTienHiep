using System.Collections.Generic;

public class ItemAmount
{
    public string itemId;
    public int amount;

    public ItemAmount(string itemId, int amount)
    {
        this.itemId = itemId;
        this.amount = amount;
    }
    public static List<ItemAmount> ParseItems(string data)
    {
        List<ItemAmount> result = new();

        if (string.IsNullOrEmpty(data))
            return result;

        string[] items = data.Split(',');

        foreach (string item in items)
        {
            string[] parts = item.Split(':');

            if (parts.Length != 2)
                continue;

            string id = parts[0];

            if (int.TryParse(parts[1], out int amount))
            {
                result.Add(new ItemAmount(id, amount));
            }
        }

        return result;
    }
    public static int GetTrucCoDan(string data)
    {
        var items = ParseItems(data);
        int count = 0;
        foreach (var item in items)
        {
            if (item.itemId == "ID_DANDUOC_TRUCCODAN_00001")
                return item.amount;
        }
        return count;
    }
}
