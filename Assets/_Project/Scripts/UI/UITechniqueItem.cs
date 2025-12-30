using UnityEngine;
using UnityEngine.UI;
using System;


public class UITechniqueItem : UIItemSlotBase, ILockable
{
    public Action<InventoryItem, InventoryItem> OnEquippedChanged;
    [SerializeField] private Image lockIcon;
    public bool IsLocked => isLocked;
    private bool isLocked = true;
    protected override void Awake()
    {
        base.Awake();
        navigation = GetComponent<NavigationTechniqueDetail>();
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
        if(IsLocked)
            return false;
        if (ctx.ItemOfFrom.data is TechniqueData)
        {
            return true;
        }
        return false;
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        lockIcon.gameObject.SetActive(false);
        isLocked = false;
    }
}
