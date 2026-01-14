

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class ShopPageManager : TGTHMonoBehaviour
{
    [SerializeField] private ShopPagePresenter presenter;
    [SerializeField] private List<InventoryItem> listItemDatas;
    private List<InventoryItem> listItemsPurchased = new List<InventoryItem>();
    public bool isAwake = false;
    protected override void Awake()
    {
        base.Awake();
        isAwake = true;
    }
    public void SetInventoryData(List<InventoryItem> items)
    {
        listItemDatas = items;
        listItemDatas.AddRange(listItemsPurchased);
        presenter?.SetInventoryData(listItemDatas);
    }
    private void OnEnable()
    {
        presenter?.Refesh();    
    }
    public bool AddItemData(ItemData data)
    {
        return true;
    }
    private bool RemoveItemData(ItemData data)
    {
        return true;
    }
    public bool RemoveInventoryItem(InventoryItem item)
    {
        return true;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        presenter = GetComponent<ShopPagePresenter>();
    }
}