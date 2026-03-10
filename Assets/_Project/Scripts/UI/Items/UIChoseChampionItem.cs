using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;


public class UIChoseChampionItem : UIItemSlotBase
{
    public Vector2Int championIndex = new Vector2Int(0, 0);
    [SerializeField] private Image emptySlot;
    public event Action<UIChoseChampionItem> OnEmptySlotClicked;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    public override void ResetData()
    {
        base.ResetData();
        emptySlot.gameObject.SetActive(true);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (HasItem() == false)
        {
            OnEmptySlotClicked?.Invoke(this);
        }
    }
    public override bool HasItem()
    {
        return inventoryItem != null;
    }
    public override void SetItem(InventoryItem newItem)
    {
        inventoryItem = newItem;
        if (inventoryItem == null)
        {
            ResetData();
            return;
        }
        emptySlot.gameObject.SetActive(false);
        SetData(
            inventoryItem.data.itemIcon,
            inventoryItem.stackSize
        );
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
