using UnityEngine;


public class UIEquipmentSlot : UIItemSlotBase
{
    public EquipmentType equipmentType;
    protected override void Awake()
    {
        base.Awake();
        uiInventoryType = UIInventoryType.Equipment;
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
        // chỉ nhận item equipment đúng slot
        if (ctx.ItemOfFrom.data is ItemEquitmentData eq)
            return eq.equipmentType == equipmentType;

        return false;
    }
}
