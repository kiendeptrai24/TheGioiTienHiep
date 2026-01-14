

using System;
using System.Collections.Generic;
using UnityEngine;
public class InventoryUseSystem : TGTHMonoBehaviour, IUsable
{
    [SerializeField] private InventoryPageManager inventoryPageManager;
    [SerializeField] private TechniquePageManager techniqueManager;
    [SerializeField] private SkillPageManager skillPageManager;
    public List<ItemData> itemUsed = new List<ItemData>();

    public void UseItem(UIItemSlotBase uiItem, int quantity = 1)
    {
        if (TryAddItemToPages(uiItem.inventoryItem))
        {
            var inventoryItem = uiItem.inventoryItem;

            itemUsed.Add(inventoryItem.data);
            inventoryPageManager.RemoveInventoryItem(inventoryItem);
            uiItem.ResetData();
        }
        else
        {
            Debug.Log("dont have use item");
        }
    }
    private bool TryAddItemToPages(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
            return false;

        if (techniqueManager.AddItemData(inventoryItem.data))
            return true;

        if (skillPageManager.AddItemData(inventoryItem.data))
            return true;

        return false;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        inventoryPageManager = GetComponent<InventoryPageManager>();

    }
}