using UnityEngine;

/// <summary>
/// Linh Thạch resource
/// </summary>
public class LinhThachResource : IResourceValidator
{
    private int requiredAmount;

    public LinhThachResource(int required = 0)
    {
        requiredAmount = required;
    }

    public bool CanUse(PlayerResource playerResource, ItemData itemData)
    {
        if (playerResource == null)
            return false;

        // Kiểm tra số lượng linh thạch
        bool hasEnoughResource = playerResource.linhThach >= requiredAmount;

        // Kiểm tra itemData nếu có yêu cầu
        // bool itemDataValid = itemData == null || itemData.itemId != null;
        Debug.Log($"Linh thạch hiện tại: {playerResource.linhThach}, yêu cầu: {requiredAmount}, đủ điều kiện: {hasEnoughResource}");
        return hasEnoughResource;
    }

    public string GetResourceName() => "Linh Thạch";

    public int GetCurrentAmount(PlayerResource playerResource)
        => playerResource?.linhThach ?? 0;

    public void Consume(PlayerResource playerResource, int amount)
    {
        if (playerResource != null)
        {
            playerResource.linhThach -= amount;
            if (playerResource.linhThach < 0)
                playerResource.linhThach = 0;
        }
    }

    public int GetRequiredAmount() => requiredAmount;
}
