/// <summary>
/// Yếu Dần resource
/// </summary>
public class YeuDanResource : IResourceValidator
{
    private int requiredAmount;

    public YeuDanResource(int required = 0)
    {
        requiredAmount = required;
    }

    public bool CanUse(PlayerResource playerResource, ItemData itemData)
    {
        if (playerResource == null)
            return false;

        // Kiểm tra số lượng yếu dần
        bool hasEnoughResource = playerResource.yeuDan >= requiredAmount;

        // Kiểm tra itemData nếu có yêu cầu
        bool itemDataValid = itemData == null || itemData.itemId != null;

        return hasEnoughResource && itemDataValid;
    }

    public string GetResourceName() => "Yêu Đan";

    public int GetCurrentAmount(PlayerResource playerResource)
        => playerResource?.yeuDan ?? 0;

    public void Consume(PlayerResource playerResource, int amount)
    {
        if (playerResource != null)
        {
            playerResource.yeuDan -= amount;
            if (playerResource.yeuDan < 0)
                playerResource.yeuDan = 0;
        }
    }
}
