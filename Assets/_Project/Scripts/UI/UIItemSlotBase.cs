using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
public enum UIInventoryType
{
    Inventory,
    Equipment,
}
/// <summary>
/// Base abstract class for all item UI slots
/// (Inventory, Equipment, Storage, Shop...)
/// </summary>
public abstract class UIItemSlotBase : TGTHMonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IDropHandler,
    IDragHandler
{
    public ActionNavigation navigation;
    [SerializeField] private bool resetDataOnAwake = true;
    [SerializeField] protected UIInventoryType uiInventoryType;
    [Header("UI References")]
    [SerializeField] protected Image itemImage;
    [SerializeField] protected Image borderImage;
    public InventoryItem inventoryItem;
    protected bool empty = true;

    #region Events
    public event Action<UIItemSlotBase> OnItemClicked;
    public event Action<UIItemSlotBase> OnItemDroppedOn;
    public event Action<UIItemSlotBase> OnItemBeginDrag;
    public event Action<UIItemSlotBase> OnItemEndDrag;
    public event Action<UIItemSlotBase> OnRightMouseBtnClick;
    #endregion

    protected override void Awake()
    {
        if(resetDataOnAwake)
            ResetData();
        Deselect();
    }
    public UIInventoryType GetUIInventoryType()
    {
        return uiInventoryType;
    }
    #region Core UI Logic
    public virtual void ResetData()
    {
        empty = true;
        itemImage.gameObject.SetActive(false);
        inventoryItem = null;
    }
    public virtual void SetData(Sprite sprite, int quantity)
    {
        empty = false;
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;

    }


    public virtual void SwapWith(UIItemSlotBase other)
    {
        var temp = inventoryItem;
        SetItem(other.inventoryItem);
        other.SetItem(temp);
    }
    public virtual void Select()
    {
        borderImage.enabled = true;
    }

    public virtual void Deselect()
    {
        borderImage.enabled = false;
    }
    #endregion

    #region Pointer Events
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (empty) return;
        if (eventData.button == PointerEventData.InputButton.Right)
            OnRightMouseBtnClick?.Invoke(this);
        else
            OnItemClicked?.Invoke(this);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (empty) return;
        OnItemBeginDrag?.Invoke(this);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        OnItemDroppedOn?.Invoke(this);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        // optional override
    }
    #endregion

    #region Abstract
    public abstract bool CanReceive(ItemDragContext ctx);
    public abstract void SetItem(InventoryItem newItem);
    public abstract bool HasItem();
    #endregion
}
