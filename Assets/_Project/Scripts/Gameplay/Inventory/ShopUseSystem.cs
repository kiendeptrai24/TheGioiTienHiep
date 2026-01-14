
using System;
using UnityEngine;

public class ShopUseSystem : TGTHMonoBehaviour, IUsable
{
    private ShopPageManager shopPageManager;
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
        var isSuccess = inventoryPageManager.AddItemData(inventoryItem.data, quantity);
        if (isSuccess)
        {
            if(shopPageManager.RemoveInventoryItem(inventoryItem) == false)
            {
                inventoryPageManager.RemoveInventoryItem(inventoryItem);
            }
        }
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        shopPageManager = GetComponent<ShopPageManager>();
    }
}