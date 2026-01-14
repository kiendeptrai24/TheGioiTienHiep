

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class InventoryPageManager : TGTHMonoBehaviour
{
    [SerializeField] private InventoryPagePresenter presenter;
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
    public bool AddItemData(ItemData data, int quantity = 1)
    {
        if (data == null)
        {
            return false;
        }
        else
        {
            List<InventoryItem> list = isAwake ? listItemDatas : listItemsPurchased;
            var item = list.Find(i => i.data.itemId == data.itemId);
            if (item != null)
            {
                if (item.CanStack())
                {
                    item.AddStack(quantity);
                }
                else
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        list.Add(new InventoryItem(data));
                    }
                }
            }
            presenter?.Refesh();
            return true;
        }
    }
    public bool AddInventoryItem(InventoryItem item)
    {
        if (item == null)
        {
            return false;
        }
        else
        {
            List<InventoryItem> list = isAwake ? listItemDatas : listItemsPurchased;
            list.Add(item);
            presenter?.Refesh();

            return true;
        }
    }
    private bool RemoveItemData(ItemData data)
    {
        if (data == null)
            return false;

        List<InventoryItem> list = isAwake ? listItemDatas : listItemsPurchased;

        var item = list.Find(i => i.data == data);
        if (item == null)
            return false;

        list.Remove(item);
        presenter?.Refesh();
        return true;
    }
    public bool RemoveInventoryItem(InventoryItem item, int quantity = 1)
    {
        List<InventoryItem> list = isAwake ? listItemDatas : listItemsPurchased;

        if (list.Contains(item))
        {
            item.RemoveStack(quantity);
            if (item.stackSize == 0)
                list.Remove(item);

            presenter?.Refesh();
            return true;
        }
        return false;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        presenter = GetComponent<InventoryPagePresenter>();
    }
}