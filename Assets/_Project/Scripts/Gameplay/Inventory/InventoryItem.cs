using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] public ItemData data;
    public int stackSize;
    private bool canStack;
    public InventoryItem(ItemData _newItem)
    {
        canStack = _newItem.canStack;
        data = _newItem;
        stackSize = 1;
    }
    public InventoryItem(ItemData _newIte, int _quantity)
    {
        canStack = _newIte.canStack;
        data = _newIte;
        stackSize = _quantity;
    }

    public void AddStack(int quantity = 1)
    {
        if (CanStack() == false) return;
        if (stackSize + quantity > 99)
            stackSize = 99;
        else
            stackSize += quantity;
        data.currentstack = stackSize;
    }
    public void RemoveStack(int quantity = 1)
    {
        if (CanStack() == false) return;
        if (stackSize - quantity < 0)
            stackSize = 0;
        else
            stackSize -= quantity;
        data.currentstack = stackSize;
    }
    public bool CanStack() => canStack;
}