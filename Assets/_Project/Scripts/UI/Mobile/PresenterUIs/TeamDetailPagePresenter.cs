


using System;
using System.Collections.Generic;
using TGTH.Mobile;
using UnityEngine;

public class TeamDetailPagePresenter : TGTHMonoBehaviour
{
    [SerializeField] private TeamDetailPageView view;
    [SerializeField] private ChooseHeroPresenter chooseHeroPresenter;
    [SerializeField] private List<InventoryItem> listItemDatas = new();
    public event Action<InventoryItem> OnSwapItemRequested;
    private UIItemSlotBase currentItemSelect;
    public List<ItemPreset> GetDatas;
    private int currentlyDraggedItemIndex = -1;
    private bool isDraging;

    protected override void Awake()
    {
        base.Awake();
        InitializeInventoryUI();
    }

    private void InitializeInventoryUI()
    {

        foreach (var uiItem in view.listOfUIItems)
        {
            var item = uiItem as UIChoseChampionItem;
            if (item == null)
                continue;
            item.OnItemClicked += HandleItemClicked;
            item.OnItemBeginDrag += HandleBeginDrag;
            item.OnItemDroppedOn += HandleItemDropped;
            item.OnItemEndDrag += HandleEndDrag;
            item.OnEmptySlotClicked += HandleEmptySlotClicked;
        }
        for (int i = 0; i < GetDatas.Count; i++)
        {
            if (i < view.listOfUIItems.Count)
            {
                view.listOfUIItems[i].SetItem(new InventoryItem(GetDatas[i].GetItemData()));
            }
        }
    }
    public void AddItem(InventoryItem item)
    {
        listItemDatas.Add(item);
    }
    private void HandleEmptySlotClicked(UIChoseChampionItem item)
    {
        chooseHeroPresenter.ChooseItem(item);
        item?.navigation.OnClick();
    }

    private void HandleEndDrag(UIItemSlotBase uiItem)
    {
        isDraging = false;
        ResetDrag();
    }
    public void AddItem(ItemData itemData, Vector2 index)
    {
        for (int i = 0; i < view.listOfUIItems.Count; i++)
        {
            if (view.listOfUIItems[i].HasItem())
                continue;

            var itemChoseChampion = view.listOfUIItems[i] as UIChoseChampionItem;

            if (itemChoseChampion == null)
                continue;

            if (itemChoseChampion.championIndex != index)
                continue;

            view.listOfUIItems[i].SetItem(new InventoryItem(itemData));
            break;
        }
        var item = new InventoryItem(itemData);
        listItemDatas.Add(item);
    }
    public void SwapItem(ItemData itemData, Vector2 index)
    {
        for (int i = 0; i < view.listOfUIItems.Count; i++)
        {
            var itemChoseChampion = view.listOfUIItems[i] as UIChoseChampionItem;

            if (itemChoseChampion == null)
                continue;

            if (itemChoseChampion.championIndex != index)
                continue;
            OnSwapItemRequested?.Invoke(view.listOfUIItems[i].inventoryItem);
            view.listOfUIItems[i].SetItem(new InventoryItem(itemData));
            break;
        }

        var item = new InventoryItem(itemData);
        listItemDatas.Add(item);
    }
    private void HandleItemDropped(UIItemSlotBase uiItem)
    {
        if (currentlyDraggedItemIndex == -1) return;
        int dropIndex = view.listOfUIItems.IndexOf(uiItem);

        if (dropIndex == -1) return;

        SwapItemsUI(currentlyDraggedItemIndex, dropIndex);
        ItemClicked(uiItem);
    }

    private void HandleBeginDrag(UIItemSlotBase uiItem)
    {
        isDraging = true;
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        currentlyDraggedItemIndex = index;

        var item = uiItem.inventoryItem;
        if (item == null) return;

        view.ToggleMouseFollower(true);
        view.SetFollowerData(item.data.itemIcon, item.data.currentstack);
    }
    private void ItemClicked(UIItemSlotBase uiItem)
    {
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        view.DeselectItem(currentItemSelect);
        view.SelectUIItem(currentItemSelect, uiItem);

        currentItemSelect = uiItem;
        ResetDrag();
    }
    private void SwapItemsUI(int from, int to)
    {
        var fromSlot = view.listOfUIItems[from];
        var toSlot = view.listOfUIItems[to];
        if (fromSlot == toSlot)
        {
            ResetDrag();
            return;
        }

        fromSlot.SwapWith(toSlot);
    }
    private void HandleItemClicked(UIItemSlotBase uiItem)
    {
        if (isDraging)
        {
            isDraging = false;
            return;
        }
        if (uiItem.HasItem()) return;
        uiItem?.navigation.OnClick();
    }
    private void ResetDrag()
    {
        view.ToggleMouseFollower(false);
        currentlyDraggedItemIndex = -1;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        view = GetComponent<TeamDetailPageView>();
    }
}