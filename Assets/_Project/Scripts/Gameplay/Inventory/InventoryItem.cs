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
        stackSize = data.currentstack;
    }
    public void AddStack()
    {
        stackSize++;
        data.currentstack = stackSize;
    }
    public void RemoveStack() 
    {
        stackSize--;
        data.currentstack = stackSize;
    }
}