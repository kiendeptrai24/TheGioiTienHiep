
using UnityEngine;

public abstract class UIItemSlotLockable : UIItemSlotBase
{
    protected bool isLocked = true;
    public override bool CanReceive(ItemDragContext ctx)
    {
        if (isLocked) return false;
        return true;
    }
    public override abstract bool HasItem();
    public virtual void Lock() => isLocked = true;
    public virtual void Unlock() => isLocked = false;
    public bool IsLocked() => isLocked;
    public override abstract void SetItem(InventoryItem newItem);
}