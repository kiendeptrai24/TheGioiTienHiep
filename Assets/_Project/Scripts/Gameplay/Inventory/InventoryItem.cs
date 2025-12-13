using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] public ItemData data;
    public int stackSize;
    public InventoryItem(ItemData _newItem)
    {
        data = _newItem;
        AddStack();
    }
    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}