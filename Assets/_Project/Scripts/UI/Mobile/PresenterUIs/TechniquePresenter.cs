

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    public class TechniquePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private TechniquePageView view;
        [SerializeField] private ItemTechniqueDetailPageView itemTechniqueDetailPageView;
        private TechniqueSystem techniqueSystem;
        private InventoryCenterManager inventoryCenterManager;
        private List<InventoryItem> listItemDatas = new List<InventoryItem>();
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;
        private bool isSWapped = false;
        protected override void Awake()
        {
            view.ToggleMouseFollower(false);
            InitializeInventoryUI(50);
            inventoryCenterManager = InventoryCenterManager.Instance;
            inventoryCenterManager.OnItemTechniqueDataChanged += SetItemData;
            SetItemData(inventoryCenterManager.GetDataType(ItemType.Technique));
        }

        public void UnlockItem(int count)
        {
            for (int i = 0; i < view.listOfEquitmentItems.Count; i++)
            {
                if (i >= count)
                    break;
                view.listOfEquitmentItems[i].Unlock();
            }
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
            ShowAllItems();
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
                if (item is UITechniqueItem uiTechniqueSlot)
                {
                    uiTechniqueSlot.OnEquippedChanged += HandleEquippedChanged;
                }
            }
        }
        public void SetEquipmentSystem(TechniqueSystem system)
        {
            techniqueSystem = system;
        }
        private void HandleEquippedChanged(InventoryItem item1, InventoryItem item2)
        {
            if (techniqueSystem == null) return;
            techniqueSystem.Unequip(item1);
            techniqueSystem.Equip(item2);
        }

        public void ShowAllItems()
        {
            view.ShowInventory(listItemDatas);
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
            itemTechniqueDetailPageView.HandleItemClicked(uiItem.inventoryItem);
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

        public void AddItem()
        {
            if (currentItemSelect == null) return;
            currentItemSelect.inventoryItem.AddStack();
            currentItemSelect.SetItem(currentItemSelect.inventoryItem);
        }
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