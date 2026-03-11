using UnityEngine;
using UnityEngine.UI;
using System;


public class UISkillItem : UIItemSlotLockable
{
    public int skillIndex;
    [SerializeField] private Image emptySlot;
    [SerializeField] private Image lockIcon;
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
        if (IsLocked() == false)
        {
            emptySlot.gameObject.SetActive(true);
            lockIcon.gameObject.SetActive(false);
        }
        else
        {
            emptySlot.gameObject.SetActive(false);
            lockIcon.gameObject.SetActive(true);
        }
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
        if (base.CanReceive(ctx) == false)
            return false;
        if (ctx.ItemOfFrom.data is SkillData)
        {
            return true;
        }
        return false;
    }

    public override void Lock()
    {
        base.Lock();
        lockIcon.gameObject.SetActive(true);
        emptySlot.gameObject.SetActive(false);
    }

    public override void Unlock()
    {
        base.Unlock();
        lockIcon.gameObject.SetActive(false);
        emptySlot.gameObject.SetActive(true);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        navigation = GetComponent<NavigationSkillDetail>();
    }
}
