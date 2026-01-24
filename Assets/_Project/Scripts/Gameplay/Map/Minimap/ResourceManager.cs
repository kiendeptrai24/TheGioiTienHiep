
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
    protected override void Start()
    {
        base.Start();
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