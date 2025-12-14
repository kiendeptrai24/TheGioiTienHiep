using UnityEngine;

/// <summary>
/// Inventory slot implementation
/// </summary>
public class UIInventoryItem : UIItemSlotBase
{
    protected override void Awake()
    {
        base.Awake();
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
            inventoryItem.stackSize,
            inventoryItem.data.itemName
        );
    }

    public override bool CanReceive(ItemDragContext ctx)
    {
        if(ctx.From.GetUIInventoryType() == ctx.To.GetUIInventoryType() 
            && ctx.From.GetUIInventoryType() == UIInventoryType.Inventory)
            return true;
        
        if(ctx.ItemOfTo != null)
        {
            if(ctx.ItemOfTo.data is ItemEquitmentData eq)
            {
                var eqItemFrom = ctx.ItemOfFrom.data as ItemEquitmentData;
                return eqItemFrom.equipmentType == eq.equipmentType;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}
