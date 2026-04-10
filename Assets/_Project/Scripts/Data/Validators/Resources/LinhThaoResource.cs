/// <summary>
/// Linh Thảo resource
/// </summary>
public class LinhThaoResource : IResourceValidator
{
    private int requiredAmount;

    public LinhThaoResource(int required = 0)
    {
        requiredAmount = required;
    }

    public bool CanUse(PlayerResource playerResource, ItemData itemData)
    {
        if (playerResource == null)
            return false;

        // Kiểm tra số lượng linh thảo
        bool hasEnoughResource = playerResource.linhThao >= requiredAmount;

        // Kiểm tra itemData nếu có yêu cầu
        bool itemDataValid = itemData == null || itemData.itemId != null;

        return hasEnoughResource && itemDataValid;
    }

    public string GetResourceName() => "Linh Thảo";

    public int GetCurrentAmount(PlayerResource playerResource)
        => playerResource?.linhThao ?? 0;

    public void Consume(PlayerResource playerResource, int amount)
    {
        if (playerResource != null)
        {
            playerResource.linhThao -= amount;
            if (playerResource.linhThao < 0)
                playerResource.linhThao = 0;
        }
    }
}
