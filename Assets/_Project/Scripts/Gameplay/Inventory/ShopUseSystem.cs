
using System;
using UnityEngine;

public class ShopUseSystem : TGTHMonoBehaviour, IUsable
{
    [SerializeField] private InventoryPageManager inventoryPageManager;
    private InventoryCenterManager inventoryCenterManager;
    protected override void Awake()
    {
        base.Awake();
        inventoryCenterManager = InventoryCenterManager.Instance;
        LoadComponent();
    }
    public void UseItem(UIItemSlotBase uiItem, int quantity = 1)
    {
        var inventoryItem = uiItem.inventoryItem;
        if (inventoryItem == null) return;
        inventoryCenterManager.AddData(inventoryItem.data, quantity);
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}