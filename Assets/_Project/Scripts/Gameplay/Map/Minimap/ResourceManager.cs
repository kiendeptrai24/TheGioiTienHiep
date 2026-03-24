
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : TGTHMonoBehaviour
{
    [SerializeField] private List<ItemMapWorld> items;
    [SerializeField] private List<ItemData> itemResources;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        foreach (var item in items)
        {
            itemResources.Add(item.GetItemData());
        }
    }
    public void AddItemMapWorld(ItemMapWorld item)
    {
        items.Add(item);
        itemResources.Add(item.GetItemData());
    }
    public void RemoveItemMapWorld(ItemMapWorld item)
    {
        items.Remove(item);
        itemResources.Remove(item.GetItemData());
    }
    public List<ItemData> GetItems()
    {
        return itemResources;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}