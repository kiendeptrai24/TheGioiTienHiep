

using System;
using System.Collections.Generic;
using UnityEngine;
public class InventoryUseSystem : TGTHMonoBehaviour
{
    [SerializeField] private TechniqueManager techniqueManager;
    [SerializeField] private SkillPageManager skillPageManager;
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public List<ItemData> itemUsed = new List<ItemData>();
    public void UseItem(UIItemSlotBase uiItem)
    {
        var inventoryItem = uiItem.inventoryItem;
        if (inventoryItem == null) return;
        bool successAddItem = techniqueManager.AddItemData(inventoryItem.data) || skillPageManager.AddItemData(inventoryItem.data);
        if (successAddItem)
        {
            Debug.Log("using item complete");
            itemUsed.Add(inventoryItem.data);
            inventoryItems.Remove(inventoryItem);
            uiItem.ResetData();
        }
        else
        {
            Debug.Log("dont have use item");
        }
    }
    public void SetInventoryData(List<InventoryItem> items)
    {
        inventoryItems = items;
    }


}