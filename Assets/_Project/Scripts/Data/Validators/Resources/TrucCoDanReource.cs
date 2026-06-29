using System.Collections.Generic;

/// <summary>
/// Yếu Dần resource
/// </summary>
public class TrucCoDanReource : IResourceValidator
{
    private const string TrucCoDanBaseId = "ID_DANDUOC_TRUCCODAN";
    public int requiredAmount;
    private string requiredItemId;

    public TrucCoDanReource(string required = "")
    {
        List<ItemAmount> itemAmounts = ItemAmount.ParseItems(required);
        foreach (ItemAmount itemAmount in itemAmounts)
        {
            if (itemAmount.itemId == TrucCoDanBaseId)
            {
                requiredItemId = itemAmount.itemId;
                requiredAmount = itemAmount.amount;
                break;
            }
        }
    }

    public bool CanUse(PlayerResource playerResource, ItemData itemData)
    {
        if (playerResource == null)
            return false;
        ItemAmount itemAmount = GetItemAmount(playerResource);
        if (itemAmount == null)
            return false;

        // Kiểm tra số lượng yếu dần
        bool hasEnoughResource = itemAmount.amount >= requiredAmount;

        // Kiểm tra itemData nếu có yêu cầu
        bool itemDataValid = itemData == null || itemData.itemId != null;

        return hasEnoughResource && itemDataValid;
    }

    private ItemAmount GetItemAmount(PlayerResource playerResource)
    {
        int totalAmount = 0;
        foreach (ItemAmount item in playerResource.itemAmounts)
        {
            if (!string.IsNullOrEmpty(requiredItemId) && item.itemId == requiredItemId)
                totalAmount += item.amount;
        }

        if (!string.IsNullOrEmpty(requiredItemId))
            return totalAmount > 0 ? new ItemAmount("", requiredItemId, totalAmount) : null;

        return null;
    }

    public string GetResourceName() => "Trúc Cơ Đan";

    public int GetCurrentAmount(PlayerResource playerResource)
    {
        ItemAmount itemAmount = GetItemAmount(playerResource);
        if (itemAmount == null)
            return 0;
        return itemAmount.amount;
    }

    public void Consume(PlayerResource playerResource, int amount)
    {
        if (playerResource != null)
        {
            ItemAmount itemAmount = GetItemAmount(playerResource);
            if (itemAmount == null)
                return;

            itemAmount.amount -= amount;
            if (itemAmount.amount < 0)
                itemAmount.amount = 0;
        }
    }

    public int GetRequiredAmount() => requiredAmount;
}
