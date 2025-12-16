using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;


public class UIEquipmentSlot : UIItemSlotBase
{
    public EquipmentType equipmentType;
    [SerializeField] private Image emtpySlot;
    public Action<InventoryItem, InventoryItem> OnEquippedChanged;
    protected override void Awake()
    {
        base.Awake();
        uiInventoryType = UIInventoryType.Equipment;
    }
    public override void ResetData()
    {
        base.ResetData();
        emtpySlot.gameObject.SetActive(true);
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
        emtpySlot.gameObject.SetActive(false);
        SetData(
            inventoryItem.data.itemIcon,
            inventoryItem.stackSize
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
