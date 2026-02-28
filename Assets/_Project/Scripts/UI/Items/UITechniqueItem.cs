using UnityEngine;
using UnityEngine.UI;
using System;


public class UITechniqueItem : UIItemSlotLockable
{
    public Action<InventoryItem, InventoryItem> OnEquippedChanged;
    [SerializeField] private Image lockIcon;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        uiInventoryType = UIInventoryType.Equipment;
    }
    public override void ResetData()
    {
        base.ResetData();
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
        SetData(
            inventoryItem.data.itemIcon,
            inventoryItem.stackSize
        );
    }
    public override bool CanReceive(ItemDragContext ctx)
    {
        if(base.CanReceive(ctx) == false)
            return false;
        if (ctx.ItemOfFrom.data is TechniqueData)
        {
            return true;
        }
        return false;
    }
    public override void Lock()
    {
        base.Lock();
        lockIcon.gameObject.SetActive(true);
    }

    public override void Unlock()
    {
        base.Unlock();
        lockIcon.gameObject.SetActive(false);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        navigation = GetComponent<NavigationTechniqueDetail>();
    }
}
