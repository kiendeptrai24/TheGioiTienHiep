/// <summary>
/// Khoáng Thạch resource
/// </summary>
public class KhoangThachResource : IResourceValidator
{
    private int requiredAmount;

    public KhoangThachResource(int required = 0)
    {
        requiredAmount = required;
    }

    public bool CanUse(PlayerResource playerResource, ItemData itemData)
    {
        if (playerResource == null)
            return false;

        // Kiểm tra số lượng khoáng thạch
        bool hasEnoughResource = playerResource.khoangThach >= requiredAmount;

        // Kiểm tra itemData nếu có yêu cầu
        bool itemDataValid = itemData == null || itemData.itemId != null;

        return hasEnoughResource && itemDataValid;
    }

    public string GetResourceName() => "Khoáng Thạch";

    public int GetCurrentAmount(PlayerResource playerResource)
        => playerResource?.khoangThach ?? 0;

    public void Consume(PlayerResource playerResource, int amount)
    {
        if (playerResource != null)
        {
            playerResource.khoangThach -= amount;
            if (playerResource.khoangThach < 0)
                playerResource.khoangThach = 0;
        }
    }

    public int GetRequiredAmount() => requiredAmount;
}
