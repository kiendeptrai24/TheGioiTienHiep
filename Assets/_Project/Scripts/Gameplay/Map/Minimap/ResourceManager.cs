
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMap.Travel;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] private List<ItemMapWorld> items;
    [SerializeField] private List<ItemData> itemResources;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
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
    public List<ItemData> GetItemsRange(Vector3 position, float range)
    {
        var result = new List<ItemData>();

        Collider[] colliders = Physics.OverlapSphere(position, range);

        foreach (var col in colliders)
        {
            var itemWorld = col.GetComponent<ItemMapWorld>();
            if (itemWorld != null)
            {
                result.Add(itemWorld.GetItemData());
            }
        }
        return result;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}