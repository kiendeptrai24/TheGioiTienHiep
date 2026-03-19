
using System;
using UnityEngine;

public class ShopUseSystem : TGTHMonoBehaviour, IUsable
{
    [SerializeField] private InventoryPageManager inventoryPageManager;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    public void UseItem(UIItemSlotBase uiItem, int quantity = 1)
    {
        var inventoryItem = uiItem.inventoryItem;
        if (inventoryItem == null) return;
        inventoryPageManager.AddItemData(inventoryItem.data, quantity);
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}