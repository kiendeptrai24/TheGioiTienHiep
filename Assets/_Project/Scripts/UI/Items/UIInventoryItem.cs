using TMPro;
using UnityEngine;

/// <summary>
/// Inventory slot implementation
/// </summary>
public class UIInventoryItem : UIItemSlotBase
{
    [SerializeField] protected TextMeshProUGUI quantityTxt;
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
        if (quantityTxt == null) return;
        quantityTxt.text = string.Empty;
    }
    public override void SetData(Sprite sprite, int quantity)
    {
        base.SetData(sprite, quantity);
        if (quantityTxt == null) return;
        quantityTxt.text = quantity > 1 ? quantity.ToString() : string.Empty;
    }
    public override bool CanReceive(ItemDragContext ctx)
    {
        if (ctx.From.GetUIInventoryType() == ctx.To.GetUIInventoryType()
            && ctx.From.GetUIInventoryType() == UIInventoryType.Inventory)
            return true;

        if (ctx.ItemOfTo != null)
        {
            if (ctx.ItemOfTo.data is EquipmentData eq)
            {
                var eqItemFrom = ctx.ItemOfFrom.data as EquipmentData;
                return eqItemFrom.equipmentType == eq.equipmentType;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        navigation = GetComponent<ActionNavigation>();
    }
}
