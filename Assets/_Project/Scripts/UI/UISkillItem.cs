using UnityEngine;
using UnityEngine.UI;
using System;


public class UISkillItem : UIItemSlotBase, ILockable
{
    public int skillIndex;
    [SerializeField] private Image emptySlot;
    [SerializeField] private Image lockIcon;
    public Action<InventoryItem, InventoryItem> OnEquippedChanged;

    public bool IsLocked => isLocked;
    private bool isLocked = true;
    protected override void Awake()
    {
        base.Awake();
        navigation = GetComponent<NavigationSkillDetail>();
        uiInventoryType = UIInventoryType.Equipment;
    }
    public override void ResetData()
    {
        base.ResetData();
        //emptySlot.gameObject.SetActive(false);
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
        if (IsLocked)
            return false;
        if (ctx.ItemOfFrom.data is SkillData skill)
        {
            return true;
        }
        return false;
    }

    public void Lock()
    {
        lockIcon.gameObject.SetActive(true);
        emptySlot.gameObject.SetActive(false);
        isLocked = true;
    }

    public void Unlock()
    {
        lockIcon.gameObject.SetActive(false);
        emptySlot.gameObject.SetActive(true);
        isLocked = false;
    }
}
