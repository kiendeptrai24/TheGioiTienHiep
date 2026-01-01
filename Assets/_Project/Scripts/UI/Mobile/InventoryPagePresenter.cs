using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    public class InventoryPagePresenter : TGTHMonoBehaviour, IPointerClickHandler, IEndDragHandler
    {
        [SerializeField] private InventoryPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        [SerializeField] private InventoryUseSystem inventoryUseSystem;
        private List<InventoryItem> listItemDatas;
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;

        protected override void Awake()
        {
            view.OnRefreshClicked += ShowItem;
            view.OnSortClicked += SortInventory;

            view.ToggleMouseFollower(false);
            InitializeInventoryUI(50);
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
        public void SortInventory()
        {
            int type = view.itemtypeDrop.value;
            int quality = view.qualityTypeDrop.value;
            Debug.Log(type);
            // Lấy enum từ dropdown value
            ItemType selectedType = (ItemType)type;
            QualityType selectedQuality = (QualityType)quality;
            Debug.Log(selectedType.ToString());
            Debug.Log(selectedQuality.ToString());
            // Lọc và sắp xếp danh sách
            var sortedList = listItemDatas
                .Where(item => (item.data.itemType == selectedType) && (item.data.qualityType == selectedQuality))
                .OrderBy(item => item.data.itemType)
                .ThenByDescending(item => item.data.qualityType)
                .ToList();

            if (sortedList.Count == 0)
                sortedList = new();

            view.ShowAllItems(sortedList);

        }
        private void HandleItemClicked(UIItemSlotBase uiItem)
        {
            if (isDraging)
            {
                isDraging = false;
                return;
            }
            ItemClicked(uiItem);
            Navigation(uiItem);
        }

        private void Navigation(UIItemSlotBase uiItem)
        {
            if (uiItem.inventoryItem.data is TechniqueData)
            {
                var popup = PopupManager.Instance.GetPopup<UseItemPopup>();
                BaseSetupData data = new BaseSetupData("Bạn có muốn sử dụng công pháp này không?");

                if (popup != null)
                {
                    popup.ShowPopup(data,
                    onConfirm: (BasePopupData result) =>
                    {
                        inventoryUseSystem.UseItem(uiItem);
                    },
                    onCancel: () =>
                    {
                        // inventoryUseSystem.UseItem()
                    });
                }
                return;
            }
            else if (uiItem.inventoryItem.data is SkillData)
            {
                var popup = PopupManager.Instance.GetPopup<UseItemPopup>();
                BaseSetupData data = new BaseSetupData("Bạn có muốn sử dụng kỹ năng này không?");

                if (popup != null)
                {
                    popup.ShowPopup(data,
                    onConfirm: (BasePopupData result) =>
                    {
                        inventoryUseSystem.UseItem(uiItem);
                    },
                    onCancel: () =>
                    {
                        // inventoryUseSystem.UseItem()
                    });
                }
                return;
            }
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

            if (dropIndex == -1) return;

            SwapItemsUI(currentlyDraggedItemIndex, dropIndex);
            ItemClicked(uiItem);
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

        private void ResetDrag()
        {
            view.ToggleMouseFollower(false);
            currentlyDraggedItemIndex = -1;
        }

        private void ShowItem()
        {
            view.ShowAllItems(listItemDatas);
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
        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<InventoryPageView>();
            inventoryUseSystem = GetComponent<InventoryUseSystem>();
        }
    }
}
