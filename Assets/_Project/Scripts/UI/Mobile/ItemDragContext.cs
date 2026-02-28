public readonly struct ItemDragContext
{
    public readonly UIItemSlotBase From;
    public readonly UIItemSlotBase To;
    public readonly InventoryItem ItemOfFrom;
    public readonly InventoryItem ItemOfTo;

    public ItemDragContext(UIItemSlotBase from, UIItemSlotBase to)
    {
        From = from;
        To = to;
        ItemOfTo = To != null ? To.inventoryItem : null;
        ItemOfFrom = From != null ? From.inventoryItem : null;
    }
}
