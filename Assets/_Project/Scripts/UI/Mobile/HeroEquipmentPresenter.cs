using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    public class HeroEquipmentPresenter : IItemDetailPageView, IEndDragHandler
    {
        [SerializeField] private HeroEquitmentPageView view;
        [SerializeField] private StatsManager statsManager;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        private EquipmentSystem equipmentSystem;
        private List<InventoryItem> listItemDatas;
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;
        private bool isSWapped = false;
        private bool setup = false;
        protected override void Awake()
        {
            statsManager.OnValueChanged += UpdateDataItem;
            view.OnRefreshClicked += ShowAllItemsInInventory;
            view.OnSortClicked += SortInventory;

            view.ToggleMouseFollower(false);
            InitializeInventoryUI(50);
        }
        private void OnEnable()
        {
            ShowAllItems();
            UpdateDataItem();
        }
        public void UpdateDataItem()
        {
            if (!setup)
            {
                Init();
                setup = true;
            }
            view.ShowEquipmentItems(statsManager.data);
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
                {
                    uIEquipmentSlot.OnEquippedChanged += HandleEquippedChanged;
                }
            }
        }
        private void Init()
        {
            view.equipmentSlotsDictionary = new Dictionary<EquipmentType, UIItemSlotBase>();
            foreach (var slot in view.listOfEquitmentItems)
            {
                view.equipmentSlotsDictionary.Add(slot.equipmentType, slot);
            }
        }
        public void SetEquipmentSystem(EquipmentSystem system)
        {
            equipmentSystem = system;
        }
        private void HandleEquippedChanged(InventoryItem item1, InventoryItem item2)
        {
            if (equipmentSystem == null) return;
            equipmentSystem.Unequip(item1);
            equipmentSystem.Equip(item2);
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

        private void ShowAllItemsInInventory()
        {
            var equip = GetListItemEquipment();
            List<InventoryItem> filteredList = new();

            foreach (var item in listItemDatas)
            {
                if (!equip.Contains(item.data))
                    filteredList.Add(item);
            }

            view.ShowAllItemInInventory(filteredList);
        }

        private void SortInventory()
        {
            // get equipqment type and quality in UI
            int type = view.eqipmenttypeDrop.value + 1;
            int quality = view.qualityTypeDrop.value;

            //convert to EquipmentType and QualityType
            EquipmentType selectedType = (EquipmentType)type;
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
                    return (type == 0 || eq.equipmentType == selectedType)
                        && (quality == 0 || eq.qualityType == selectedQuality);
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
            itemDetailPageView.HandleItemClicked(uiItem.inventoryItem);
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

        public override void HandleItemClicked(InventoryItem inventoryItem)
        {
            

            if (inventoryItem == null) return;
            if (inventoryItem.data is HeroData heroData)
            {
                view.ShowEquipmentItems(heroData);
            }
        }
    }
}
