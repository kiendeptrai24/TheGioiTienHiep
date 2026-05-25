using System.Collections.Generic;

/// <summary>
/// Yếu Dần resource
/// </summary>
public class TrucCoDanReource : IResourceValidator
{
    private string instacnceId = "ID_DANDUOC_TRUCCODAN_00001";
    public int requiredAmount;
    private string requiredItemId;

    public TrucCoDanReource(string required = "")
    {
        List<ItemAmount> itemAmounts = ItemAmount.ParseItems(required);
        foreach (ItemAmount itemAmount in itemAmounts)
        {
            if (itemAmount.itemId == instacnceId)
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
        foreach (ItemAmount item in playerResource.itemAmounts)
        {
            if (item.itemId == instacnceId)
            {
                return item;
            }
        }

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
