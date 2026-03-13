using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;


public class UICreateCharacterItem : UIItemSlotBase
{
    [SerializeField] private Image emptySlot;
    public event Action<UIItemSlotBase> OnItemEmptySlotClicked;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        uiInventoryType = UIInventoryType.Equipment;
    }
    public override void ResetData()
    {
        base.ResetData();
        emptySlot.gameObject.SetActive(true);
    }
    public override bool HasItem()
    {
        return inventoryItem != null;
    }
    public override void SetItem(InventoryItem newItem)
    {
        var oldItem = inventoryItem;
        inventoryItem = newItem;

        if (inventoryItem == null)
        {
            ResetData();
            return;
        }
        SetData(
            inventoryItem.data.itemIcon,
            inventoryItem.stackSize
        );
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (HasItem() == false)
            OnItemEmptySlotClicked?.Invoke(this);
    }
    public override void SetData(Sprite sprite, int quantity)
    {
        base.SetData(sprite, quantity);
        emptySlot.gameObject.SetActive(false);
    }
    public override bool CanReceive(ItemDragContext ctx)
    {
        return true;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        navigation = GetComponent<ActionNavigation>();
    }
}
