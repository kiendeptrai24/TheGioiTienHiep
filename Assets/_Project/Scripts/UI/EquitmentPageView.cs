

using UnityEngine;

public class EquitmentPageView : MonoBehaviour 
{
    [SerializeField] private UIInventoryDescription itemDescription;

    public void SetItemDescription(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || inventoryItem.data == null) return; 
        var data = inventoryItem.data;
        itemDescription.SetInvenotoryItem(inventoryItem);
        itemDescription.SetDescription(data.itemIcon, data.itemName, data.itemDescription);
        itemDescription.SetButtonDescriptionEquipment();
    }
}