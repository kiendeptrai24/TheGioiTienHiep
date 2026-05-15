using TMPro;
using UnityEngine;

/// <summary>
/// Inventory slot implementation
/// </summary>
public class UIInventoryItemShop : UIItemSlotBase
{
    [SerializeField] protected TextMeshProUGUI quantityTxt;
    [SerializeField] protected TextMeshProUGUI nameTxt;
    [SerializeField] protected TextMeshProUGUI priceTxt;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        uiInventoryType = UIInventoryType.Inventory;

    }
    public override bool HasItem()
    {
        return inventoryItem != null;
    }

    public override void SetItem(InventoryItem newItem)
    {
        inventoryItem = newItem;

        if (inventoryItem == null)
        {
            ResetData();
            return;
        }

        SetData(
            inventoryItem.data.itemIcon,
            inventoryItem.stackSize
        );
    }
    public override void ResetData()
    {
        base.ResetData();
        quantityTxt.text = string.Empty;
        nameTxt.text = string.Empty;
        priceTxt.text = string.Empty;
    }
    public override void SetData(Sprite sprite, int quantity)
    {
        base.SetData(sprite, quantity);
        quantityTxt.text = quantity > 1 ? quantity.ToString() : string.Empty;
        nameTxt.text = inventoryItem.data.itemName;
        if (inventoryItem.data.itemPrice >= 1000)
        {
            priceTxt.text = inventoryItem.data.itemPrice / 1000 + "K";
        }
        else
        {
            priceTxt.text = inventoryItem.data.itemPrice.ToString();
        }

    }
    public override bool CanReceive(ItemDragContext ctx)
    {
        return false;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        navigation = GetComponent<ActionNavigation>();
    }
}
