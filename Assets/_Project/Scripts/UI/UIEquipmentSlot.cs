using UnityEngine;
using UnityEngine.UI;
using System;


public class UIEquipmentSlot : UIItemSlotBase
{
    public EquipmentType equipmentType;
    [SerializeField] private Image emptySlot;
    public Action<InventoryItem, InventoryItem> OnEquippedChanged;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        uiInventoryType = UIInventoryType.Equipment;
    }
    public override void ResetData()
    {
        base.ResetData();
        emptySlot.gameObject.SetActive(true);
    }
    public override bool HasItem()
    {
        return inventoryItem != null;
    }
    public override void SetItem(InventoryItem newItem)
    {
        var oldItem = inventoryItem;
        inventoryItem = newItem;

        OnEquippedChanged?.Invoke(oldItem, inventoryItem);

        if (inventoryItem == null)
        {
            ResetData();
            return;
        }
        emptySlot.gameObject.SetActive(false);
        SetData(
            inventoryItem.data.itemIcon,
            inventoryItem.stackSize
        );
    }
    public override bool CanReceive(ItemDragContext ctx)
    {
        // chỉ nhận item equipment đúng slot
        if (ctx.ItemOfFrom.data is EquitmentData eq)
            return eq.equipmentType == equipmentType;

        return false;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        navigation = GetComponent<NavigationItemDetail>();
    }
}
