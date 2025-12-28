using UnityEngine;
using UnityEngine.UI;
using System;


public class UISkillItem : UIItemSlotBase
{
    public EquipmentType equipmentType;
    public int skillIndex;
    [SerializeField] private Image emptySlot;
    public Action<InventoryItem, InventoryItem> OnEquippedChanged;
    protected override void Awake()
    {
        base.Awake();
        navigation = GetComponent<NavigationSkillDetail>();
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
        if (ctx.ItemOfFrom.data is SkillData skill)
        {
            return true;
        }
        return false;
    }
}
