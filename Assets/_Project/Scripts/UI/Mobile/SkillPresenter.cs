

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    public class SkillPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private SkillPageView view;
        [SerializeField] private ItemSkillDetailPageView itemSkillDetailPageView;
        private SkillSystem skillSystem;
        private List<InventoryItem> listItemDatas;
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;
        private bool isSWapped = false;
        protected override void Awake()
        {
            view.OnRefreshClicked += SortItems;

            view.ToggleMouseFollower(false);
            InitializeInventoryUI(50);
            ShowAllItems();
        }
        public void UnlockItem(int count)
        {
            for (int i = 0; i < view.listOfEquitmentItems.Count; i++)
            {
                if(i >= count)
                    break;
                view.listOfEquitmentItems[i].Unlock();
            }
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
                if (item is UISkillItem uiSkillSlot)
                {
                    uiSkillSlot.OnEquippedChanged += HandleEquippedChanged;
                }
            }
        }
        public void SetEquipmentSystem(SkillSystem system)
        {
            skillSystem = system;
        }
        private void HandleEquippedChanged(InventoryItem item1, InventoryItem item2)
        {
            if (skillSystem == null) return;
            skillSystem.Unequip(item1);
            skillSystem.Equip(item2);
        }
        public void Refesh()
        {
            view.RefreshInventory(listItemDatas);
        }
        public void SetInventoryData(List<InventoryItem> items)
        {
            listItemDatas = items;
            ShowAllItems();
        }
        public void SetSkillData(InventoryItem items)
        {
            listItemDatas.Add(items);
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
            if (isDraging)
            {
                isDraging = false;
                return;
            }
            ItemClicked(uiItem);
            uiItem?.navigation.OnClick();
        }
        private void ItemClicked(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;

            view.DeselectItem(currentItemSelect);
            view.SelectUIItem(currentItemSelect, uiItem);

            currentItemSelect = uiItem;
            ResetDrag();
            itemSkillDetailPageView.HandleItemClicked(uiItem.inventoryItem);
        }

        private void HandleItemRightClick(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;

            OnItemActionRequested?.Invoke(index);
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

            OnStartDragging?.Invoke(index);
        }

        private void HandleEndDrag(UIItemSlotBase uiItem)
        {
            isDraging = false;
            ResetDrag();
        }

        private void HandleItemDropped(UIItemSlotBase uiItem)
        {
            if (currentlyDraggedItemIndex == -1) return;
            int dropIndex = view.listOfUIItems.IndexOf(uiItem);
            UIItemSlotBase curUIItem = view.listOfUIItems[currentlyDraggedItemIndex];
            if (dropIndex == -1) return;

            SwapItemsUI(currentlyDraggedItemIndex, dropIndex);
            if (isSWapped)
                ItemClicked(uiItem);
            else
                ItemClicked(curUIItem);
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