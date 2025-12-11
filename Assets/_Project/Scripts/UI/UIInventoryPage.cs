using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    List<Item> itemDatas;

    [SerializeField]
    private UIInventoryItem itemPrefab;

    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private UIInventoryDescription itemDescription;

    [SerializeField]
    private MouseFollower mouseFollower;

    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    private int currentlyDraggedItemIndex = -1;

    public event Action<int> OnDescriptionRequested,
            OnItemActionRequested,
            OnStartDragging;

    public event Action<int, int> OnSwapItems;

    [SerializeField]

    private void Awake()
    {
        //Hide();
        OnSwapItems += SwapItems;
        mouseFollower.Toggle(false);
        itemDescription.ResetDescription();
        InitializeInventoryUI(50);
    }

    public void InitializeInventoryUI(int slotQuanlity)
    {
        for (int i = 0; i < slotQuanlity; i++)
        {
            UIInventoryItem uiItem =
                Instantiate(itemPrefab, contentPanel);

            listOfUIItems.Add(uiItem);
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;

            if (i >= itemDatas.Count) continue;

            Item item = new Item();
            item.itemIcon = itemDatas[i].itemIcon;
            item.itemName = itemDatas[i].itemName;
            item.itemDescription = itemDatas[i].itemDescription;
            item.maxStack = itemDatas[i].maxStack;
            uiItem.SetItem(item);
        }
    }

    internal void ResetAllItems()
    {
        foreach (var item in listOfUIItems)
        {
            item.ResetData();
            item.Deselect();
        }
    }

    internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
    {
        itemDescription.SetDescription(itemImage, name, description);
        DeselectAllItems();
        listOfUIItems[itemIndex].Select();
    }

    public void UpdateData(int itemIndex,
        Sprite itemImage, int itemQuantity)
    {
        if (listOfUIItems.Count > itemIndex)
        {
            listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
        }
    }

    private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
        {
            return;
        }
        OnItemActionRequested?.Invoke(index);
    }

    private void HandleEndDrag(UIInventoryItem inventoryItemUI)
    {
        ResetDraggedItem();
    }

    private void HandleSwap(UIInventoryItem inventoryItemUI)
    {
        if (currentlyDraggedItemIndex == -1)
        {
            return;
        }
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
        {
            return;
        }
        OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
        HandleItemSelection(inventoryItemUI);
    }
    
    private void SwapItems(int currentItemIndex, int itemToSwapIndex)
    {
        var item = listOfUIItems[currentItemIndex].item;
        listOfUIItems[currentItemIndex].SetItem(listOfUIItems[itemToSwapIndex].item);
        listOfUIItems[itemToSwapIndex].SetItem(item);
    }

    private void ResetDraggedItem()
    {
        mouseFollower.Toggle(false);
        currentlyDraggedItemIndex = -1;
    }

    private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
            return;
        currentlyDraggedItemIndex = index;
        var item = listOfUIItems[index].item;
        if (item == null)
            return;
        CreateDraggedItem(item.itemIcon, item.maxStack);
        HandleItemSelection(inventoryItemUI);
        OnStartDragging?.Invoke(index);
    }

    public void CreateDraggedItem(Sprite sprite, int quantity)
    {
        mouseFollower.Toggle(true);
        mouseFollower.SetData(sprite, quantity);
    }

    private void HandleItemSelection(UIInventoryItem inventoryItemUI)
    {
        int index = listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
            return;
        var item = listOfUIItems[index].item;
        if (item != null)
        {
            UpdateDescription(index, item.itemIcon, item.itemName, item.itemDescription);
        }


        OnDescriptionRequested?.Invoke(index);
    }
    [ContextMenu("Sort")]
    private void SortItem()
    {
        List<Item> uIInventories = new List<Item>();
        foreach (var item in listOfUIItems)
        {
            if (item.item == null)
                continue;
            uIInventories.Add(item.item);
            item.ResetData();
            item.Deselect();
        }
        for (int i = 0; i < uIInventories.Count; i++)
        {
            listOfUIItems[i].SetItem(uIInventories[i]);
        }

    }
    public void Show()
    {
        gameObject.SetActive(true);
        ResetSelection();
    }

    public void ResetSelection()
    {
        itemDescription.ResetDescription();
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach (UIInventoryItem item in listOfUIItems)
        {
            item.Deselect();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ResetDraggedItem();
    }
}