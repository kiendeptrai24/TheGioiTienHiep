using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EquipmentBasePagePresenter : TGTHMonoBehaviour
{
    [SerializeField] protected EquipmentBasePageView view;
    [SerializeField] protected IItemDetailPageView itemDetailPageView;
    [SerializeField] protected StatsData statsManager;
    [SerializeField] protected InventoryCenterManager inventoryCenterManager;
    [SerializeField] protected List<InventoryItem> listItemDatas;
    protected UIItemSlotBase currentItemSelect;
    protected int currentlyDraggedItemIndex = -1;
    protected bool isDraging = false;
    protected bool isSWapped = false;
    public event Action<int> OnItemActionRequested;
    public event Action<int> OnStartDragging;
    private bool isShowEquipment = false;
    private bool isOwnEquipmentPage = false;
    protected override void Awake()
    {
        view.ToggleMouseFollower(false);
        InitializeInventoryUI(50);
        view.OnRefreshClicked += ShowAllItemInInventory;
        view.OnSortClicked += SortInventory;

        LoadData();
    }

    private void LoadData()
    {
        inventoryCenterManager = InventoryCenterManager.Instance;
        inventoryCenterManager.OnItemEquitmentDataChanged += SetItemData;
        SetItemData(inventoryCenterManager.GetDataType(ItemType.Equipment, true));
        isShowEquipment = true;
        view.ShowEquipmentItems(statsManager.heroData);
        isShowEquipment = false;
    }

    protected virtual void OnEnable()
    {
        ShowItemEquipment();
        isOwnEquipmentPage = true;
    }

    protected virtual void OnDisable()
    {
        isOwnEquipmentPage = false;
    }

    private void ShowItemEquipment()
    {
        isShowEquipment = true;
        view.ShowEquipmentItems(statsManager.heroData);
        isShowEquipment = false;
    }

    private void SetItemData(List<ItemData> items)
    {

        if (listItemDatas == null)
            listItemDatas = new List<InventoryItem>();
        else
            listItemDatas.Clear();
        foreach (var item in items)
        {
            listItemDatas.Add(new InventoryItem(item));
        }
        if (isOwnEquipmentPage) return;
        ShowAllItemInInventory();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void InitializeInventoryUI(int amount)
    {
        view.CreateInventorySlots(amount);

        foreach (var uiItem in view.listOfUIItems)
        {
            uiItem.OnItemClicked += HandleItemClicked;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleItemDropped;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleItemRightClick;
        }
        foreach (var item in view.listOfEquitmentItems)
        {
            if (item is UIEquipmentSlot uIEquipmentSlot)
                uIEquipmentSlot.OnEquippedChanged += HandleEquippedChanged;

            view.equipmentSlotsDictionary.Add(item.equipmentType, item);
        }
    }

    protected virtual bool HandleEquippedChanged(InventoryItem item1, InventoryItem item2)
    {
        if (isShowEquipment) return false;
        if (item1 != null && item1.data != null)
        {
            var result = inventoryCenterManager.AddData(item1.data);
            if (result)
            {
                var heroData = statsManager.heroData as HeroData;
                var equipmentData = item1.data as EquitmentData;
                heroData.equitmentDatas.Remove(equipmentData);
            }

        }
        if (item2 != null && item2.data != null)
        {
            var result = inventoryCenterManager.RemoveData(item2.data);
            if (result)
            {
                var heroData = statsManager.heroData as HeroData;
                var equipmentData = item2.data as EquitmentData;
                heroData.equitmentDatas.Add(equipmentData);
            }
        }
        return true;
    }

    private void ShowAllItemInInventory()
    {
        view.ShowAllItemInInventory(listItemDatas);
    }

    protected virtual void SortInventory()
    {
        // get equipqment type and quality in UI
        int type = view.eqipmenttypeDrop.value + 1;
        int quality = view.qualityTypeDrop.value;

        //convert to EquipmentType and QualityType
        EquipmentType selectType = (EquipmentType)type;
        QualityType selectedQuality = (QualityType)quality;

        // get equipment item 
        var equip = GetListItemEquipment();

        // create list item dont have item is equipment
        List<InventoryItem> filteredList = new();
        foreach (var item in listItemDatas)
        {
            if (!equip.Contains(item.data))
                filteredList.Add(item);
        }

        // sort item base on EquipmentType and QualityType
        var sortedList = filteredList
            .Where(inv =>
            {
                var eq = (EquitmentData)inv.data;
                return (eq.equipmentType == selectType)
                    && (eq.qualityType == selectedQuality);
            })
            .OrderBy(inv => ((EquitmentData)inv.data).equipmentType)
            .ThenByDescending(inv => ((EquitmentData)inv.data).qualityType)
            .ToList();

        // if sortlist dont have item return empty list
        if (sortedList.Count == 0)
            sortedList = new();

        // show in ui
        view.ShowAllItemInInventory(sortedList);
    }
    private HashSet<ItemData> GetListItemEquipment()
    {
        HashSet<ItemData> temp = new();
        foreach (var item in view.listOfEquitmentItems)
        {
            if (item.inventoryItem != null)
            {
                temp.Add(item.inventoryItem.data);
            }
        }
        return temp;
    }
    public void RefreshInventory()
    {
        view.ShowAllItemInInventory(listItemDatas);
    }

    protected virtual void HandleItemClicked(UIItemSlotBase uiItem)
    {
        if (isDraging)
        {
            isDraging = false;
            return;
        }
        ItemClicked(uiItem);
        uiItem?.navigation.OnClick();
    }
    protected virtual void ItemClicked(UIItemSlotBase uiItem)
    {
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        view.SelectUIItem(currentItemSelect, uiItem);

        currentItemSelect = uiItem;
        ResetDrag();
        itemDetailPageView.HandleItemClicked(uiItem.inventoryItem);
    }

    private void HandleItemRightClick(UIItemSlotBase uiItem)
    {
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        OnItemActionRequested?.Invoke(index);
    }

    protected virtual void HandleBeginDrag(UIItemSlotBase uiItem)
    {
        isDraging = true;
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        currentlyDraggedItemIndex = index;

        var item = uiItem.inventoryItem;
        if (item == null) return;

        view.ToggleMouseFollower(true);
        view.SetFollowerData(item.data.itemIcon, item.data.currentstack);

        OnStartDragging?.Invoke(index);
    }

    protected virtual void HandleEndDrag(UIItemSlotBase uiItem)
    {
        isDraging = false;
        ResetDrag();
    }

    protected virtual void HandleItemDropped(UIItemSlotBase uiItem)
    {
        if (currentlyDraggedItemIndex == -1) return;
        int dropIndex = view.listOfUIItems.IndexOf(uiItem);
        UIItemSlotBase curUIItem = view.listOfUIItems[currentlyDraggedItemIndex];
        if (dropIndex == -1) return;

        SwapItemsUI(currentlyDraggedItemIndex, dropIndex);

        var item = isSWapped ? uiItem : curUIItem;
        ItemClicked(item);
    }

    protected void SwapItemsUI(int from, int to)
    {
        var fromSlot = view.listOfUIItems[from];
        var toSlot = view.listOfUIItems[to];
        var ctx = new ItemDragContext(fromSlot, toSlot);
        if (!toSlot.CanReceive(ctx) || !toSlot.CanReceive(ctx))
        {
            isSWapped = false;
            return;
        }
        isSWapped = true;
        fromSlot.SwapWith(toSlot);
    }
    protected void ResetDrag()
    {
        view.ToggleMouseFollower(false);
        currentlyDraggedItemIndex = -1;
    }
    [ContextMenu("Add")]
    public void AddItem()
    {
        if (currentItemSelect == null) return;
        currentItemSelect.inventoryItem.AddStack();
        currentItemSelect.SetItem(currentItemSelect.inventoryItem);
    }
    [ContextMenu("Remove")]
    public void RemoveItem()
    {
        if (currentItemSelect == null) return;
        currentItemSelect.inventoryItem.RemoveStack();
        currentItemSelect.SetItem(currentItemSelect.inventoryItem);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        view.DeselectItem(currentItemSelect);
    }

    public void Show()
    {
        view.Show();
        view.DeselectAll();
    }

    public void Hide()
    {
        view.Hide();
        ResetDrag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetDrag();
    }
}