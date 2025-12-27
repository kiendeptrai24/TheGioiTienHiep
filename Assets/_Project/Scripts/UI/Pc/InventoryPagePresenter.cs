using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.PC
{
    public class InventoryPagePresenter : MonoBehaviour, IPointerClickHandler, IEndDragHandler
    {
        [SerializeField] private InventoryPageView view;

        private List<InventoryItem> listItemDatas;
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;

        public event Action<int> OnDescriptionRequested;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isSWapped = false;


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

        private void HandleItemClicked(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;

            var item = view.listOfUIItems[index].inventoryItem;
            if (item != null && uiItem is UIInventoryItem)
            {
                //view.SetDescription(item.data.itemIcon, item.data.itemName, item.data.itemDescription);
                view.SetItemDescription(item);
            }

            view.DeselectItem(currentItemSelect);
            view.SelectUIItem(currentItemSelect, uiItem);
            currentItemSelect = uiItem;

            OnDescriptionRequested?.Invoke(index);
        }

        private void HandleItemRightClick(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;
            
            OnItemActionRequested?.Invoke(index);
        }

        private void HandleBeginDrag(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;

            currentlyDraggedItemIndex = index;

            var item = uiItem.inventoryItem;
            if (item == null) return;

            view.ToggleMouseFollower(true);
            view.SetFollowerData(item.data.itemIcon, item.data.currentstack);

            OnStartDragging?.Invoke(index);
        }

        private void HandleEndDrag(UIItemSlotBase uiItem)
        {
            ResetDrag();
        }

        private void HandleItemDropped(UIItemSlotBase uiItem)
        {
            if (currentlyDraggedItemIndex == -1) return;
            int dropIndex = view.listOfUIItems.IndexOf(uiItem);
            UIItemSlotBase curUIItem = view.listOfUIItems[currentlyDraggedItemIndex];
            //

            if (dropIndex == -1) return;

            SwapItemsUI(currentlyDraggedItemIndex, dropIndex);
            if (isSWapped)
                HandleItemClicked(uiItem);
            else
                HandleItemClicked(curUIItem);
        }

        private void SwapItemsUI(int from, int to)
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

        private void ResetDrag()
        {
            view.ToggleMouseFollower(false);
            currentlyDraggedItemIndex = -1;
        }

        private void SortItems()
        {
            List<InventoryItem> tempList = new List<InventoryItem>();

            List<UIInventoryItem> inventoryItems =
                view.listOfUIItems.OfType<UIInventoryItem>().ToList();

            foreach (var slot in inventoryItems)
            {
                if (slot.inventoryItem != null)
                    tempList.Add(slot.inventoryItem);

                slot.ResetData();
                slot.Deselect();
            }

            for (int i = 0; i < tempList.Count; i++)
                view.SetItem(i, tempList[i]);
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
            view.ResetDescriptionUI();
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
}
