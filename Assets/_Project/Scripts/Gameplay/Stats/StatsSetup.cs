

using UnityEngine;

public abstract class StatsSetup : StatsSystem
{
    public abstract override void Equip(InventoryItem item);
    public abstract override void Unequip(InventoryItem item);
    public abstract void Setup();
}
