using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPagePresenter : MonoBehaviour
{
    [SerializeField] private InventoryPageView view;

    private List<InventoryItem> listItemDatas;
    private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnDescriptionRequested;
    public event Action<int> OnItemActionRequested;
    public event Action<int> OnStartDragging;
    public event Action<int, int> OnSwapItems;

    private void Awake()
    {
        view.OnRefreshClicked += SortItems;
        view.OnDescriptionToggle += HandleDescriptionToggle;

        view.ToggleMouseFollower(false);
        view.ResetDescriptionUI();

        InitializeInventoryUI(50);
    }

    private void HandleDescriptionToggle(bool isOn)
    {
        view.ShowDescriptionPanel(isOn);
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
    }

    public void SetInventoryData(List<InventoryItem> items)
    {
        listItemDatas = items;
        ShowAllItems();
    }

    private void ShowAllItems()
    {
        view.ShowAllItems(listItemDatas);
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < listItemDatas.Count; i++)
            view.SetItem(i, listItemDatas[i]);
    }

    private void HandleItemClicked(UIInventoryItem uiItem)
    {
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        var item = view.listOfUIItems[index].item;
        if (item != null)
        {
            view.SetDescription(item.data.itemIcon, item.data.itemName, item.data.itemDescription);
        }

        view.DeselectAll();
        uiItem.Select();

        OnDescriptionRequested?.Invoke(index);
    }

    private void HandleItemRightClick(UIInventoryItem uiItem)
    {
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        OnItemActionRequested?.Invoke(index);
    }

    private void HandleBeginDrag(UIInventoryItem uiItem)
    {
        int index = view.listOfUIItems.IndexOf(uiItem);
        if (index < 0) return;

        currentlyDraggedItemIndex = index;

        var item = uiItem.item;
        if (item == null) return;

        view.ToggleMouseFollower(true);
        view.SetFollowerData(item.data.itemIcon, item.data.currentstack, item.data.itemName);

        OnStartDragging?.Invoke(index);
    }

    private void HandleEndDrag(UIInventoryItem uiItem)
    {
        ResetDrag();
    }

    private void HandleItemDropped(UIInventoryItem uiItem)
    {
        if (currentlyDraggedItemIndex == -1) return;

        int dropIndex = view.listOfUIItems.IndexOf(uiItem);
        if (dropIndex == -1) return;

        OnSwapItems?.Invoke(currentlyDraggedItemIndex, dropIndex);

        // swap UI
        SwapItemsUI(currentlyDraggedItemIndex, dropIndex);

        HandleItemClicked(uiItem);
    }

    private void SwapItemsUI(int from, int to)
    {
        var temp = view.listOfUIItems[from].item;
        view.listOfUIItems[from].SetItem(view.listOfUIItems[to].item);
        view.listOfUIItems[to].SetItem(temp);
    }

    private void ResetDrag()
    {
        view.ToggleMouseFollower(false);
        currentlyDraggedItemIndex = -1;
    }

    private void SortItems()
    {
        List<InventoryItem> tempList = new List<InventoryItem>();

        foreach (var slot in view.listOfUIItems)
        {
            if (slot.item != null)
                tempList.Add(slot.item);

            slot.ResetData();
            slot.Deselect();
        }

        for (int i = 0; i < tempList.Count; i++)
            view.SetItem(i, tempList[i]);
    }

    public void Show()
    {
        view.Show();
        view.ResetDescriptionUI();
        view.DeselectAll();
    }

    public void Hide()
    {
        view.Hide();
        ResetDrag();
    }
}
