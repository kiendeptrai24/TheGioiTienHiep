/// <summary>
/// Ma Hạch resource
/// </summary>
public class MaHachResource : IResourceValidator
{
    private int requiredAmount;

    public MaHachResource(int required = 0)
    {
        requiredAmount = required;
    }

    public bool CanUse(PlayerResource playerResource, ItemData itemData)
    {
        if (playerResource == null)
            return false;

        // Kiểm tra số lượng ma hạch
        bool hasEnoughResource = playerResource.maHach >= requiredAmount;

        // Kiểm tra itemData nếu có yêu cầu
        bool itemDataValid = itemData == null || itemData.itemId != null;

        return hasEnoughResource && itemDataValid;
    }

    public string GetResourceName() => "Ma Hạch";

    public int GetCurrentAmount(PlayerResource playerResource)
        => playerResource?.maHach ?? 0;

    public void Consume(PlayerResource playerResource, int amount)
    {
        if (playerResource != null)
        {
            playerResource.maHach -= amount;
            if (playerResource.maHach < 0)
                playerResource.maHach = 0;
        }
    }
}
