
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMap.Travel;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] private List<ItemMapWorld> items;
    [SerializeField] private List<Destination> destinations;
    [SerializeField] private List<ItemData> itemResources;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        foreach (var item in items)
        {
            itemResources.Add(item.GetItemData());
            destinations.Add(item.GetDestination());
        }
    }
    public void AddItemMapWorld(ItemMapWorld item)
    {
        items.Add(item);
        itemResources.Add(item.GetItemData());
        destinations.Add(item.GetDestination());
    }
    public void RemoveItemMapWorld(ItemMapWorld item)
    {
        items.Remove(item);
        itemResources.Remove(item.GetItemData());
        destinations.Remove(item.GetDestination());
    }
    public List<Destination> GetDestination()
    {
        return destinations;
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