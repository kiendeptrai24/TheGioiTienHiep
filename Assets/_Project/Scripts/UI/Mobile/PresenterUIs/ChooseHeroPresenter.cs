using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    public class ChooseHeroPresenter : TGTHMonoBehaviour, IPointerClickHandler, IEndDragHandler
    {
        [SerializeField] private ChooseHeroPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        [SerializeField] private TeamDetailPagePresenter teamDetailPagePresenter;
        private List<InventoryItem> listItemDatas;
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;
        private UIItemSlotBase currentItem;
        [SerializeField] private InventoryCenterManager inventoryCenterManager;

        protected override void Awake()
        {
            view.OnRefreshClicked += ShowItem;
            view.OnSortClicked += SortInventory;
            view.ToggleMouseFollower(false);

            InitializeInventoryUI(50);
            ShowAllItems();
            LoadDataCenter();
        }

        private void OnLoadDataSuccessed()
        {
            view.Reset();
            SetItemData(inventoryCenterManager.GetDataType(ItemType.Champion, true));
        }

        private void LoadDataCenter()
        {
            inventoryCenterManager = InventoryCenterManager.Instance;
            inventoryCenterManager.OnItemExistingChampionDataChanged += SetItemData;
            SetItemData(inventoryCenterManager.GetDataType(ItemType.Champion, true));
            inventoryCenterManager.OnLoadDataSuccessed += OnLoadDataSuccessed;
        }

        private void SetItemData(List<ItemData> list)
        {
            var temp = new List<InventoryItem>();
            foreach (var item in list)
            {
                if (item is HeroData)
                    temp.Add(new InventoryItem(item));
            }
            listItemDatas = temp;
            ShowAllItems();
        }

        private void InitializeInventoryUI(int amount)
        {
            view.CreateInventorySlots(amount);

            foreach (var uiItem in view.listOfUIItemsAlreadyOwned)
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
            int type = view.itemtypeDrop.value + 1;
            int quality = view.qualityTypeDrop.value;
            // Lấy enum từ dropdown value
            RaceType selectedRace = (RaceType)type;
            QuanlityType selectedQuality = (QuanlityType)quality;
            // Lọc và sắp xếp danh sách
            if (listItemDatas == null || listItemDatas.Count == 0) return;
            var sortedList = listItemDatas
                .Where(item => (((HeroData)item.data).raceType == selectedRace) && (item.data.qualityType == selectedQuality))
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
            var popup = PopupManager.Instance.GetPopup<UseItemPopup>();
            BaseSetupData data = new BaseSetupData(
                $"Bạn có muốn đặt tướng \n" +
                $"<color=green>{uiItem.inventoryItem.data.itemName}</color> vào ô {(currentItem as UIChoseChampionItem).championIndex}\n"
                + "không?");

            if (popup != null)
            {
                popup.ShowPopup(data,
                onConfirm: (BasePopupData result) =>
                {
                    if (CheckChampionHasInTeam(uiItem.inventoryItem.data.instanceId))
                    {
                        TopNotificationUI.Instance.ShowNotification("Tướng đã có trong đội hình");
                        return;
                    }
                    var item = currentItem as UIChoseChampionItem;
                    if (currentItem.HasItem())
                    {
                        teamDetailPagePresenter.SwapItem(uiItem.inventoryItem.data, item.championIndex);
                    }
                    else
                    {
                        if (teamDetailPagePresenter.CheckTeamIsFull())
                        {
                            TopNotificationUI.Instance.ShowNotification("Đội hình đã đầy, không thể thêm tướng.");
                            return;
                        }
                        teamDetailPagePresenter.AddItem(uiItem.inventoryItem.data, item.championIndex);
                    }
                },
                onCancel: () =>
                {

                },
                onInfo: () =>
                {
                    ItemClicked(uiItem);
                    Navigation(uiItem);
                });
            }
        }
        public bool CheckChampionHasInTeam(string championId)
        {
            foreach (var data in inventoryCenterManager.GetDatasChampionInTeam())
            {
                if (data != null && data.instanceId == championId)
                    return true;
            }
            return false;
        }
        private void ShowPopup(UIItemSlotBase uiItem)
        {
            var popup = PopupManager.Instance.GetPopup<UseItemPopup>();
            BaseSetupData data = new BaseSetupData("Đội hình của bạn đã đầy đủ tướng cần thiết");

            if (popup != null)
            {
                popup.ShowPopup(data,
                onConfirm: (BasePopupData result) =>
                {

                },
                onCancel: () =>
                {

                },
                onInfo: () =>
                {
                    ItemClicked(uiItem);
                    Navigation(uiItem);
                });
            }
        }
        public void ChooseItem(UIItemSlotBase item)
        {
            currentItem = item;
        }
        private void Navigation(UIItemSlotBase uiItem)
        {
            uiItem?.navigation.OnClick();
        }

        private void ItemClicked(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItemsAlreadyOwned.IndexOf(uiItem);
            if (index < 0) return;

            view.DeselectItem(currentItemSelect);
            view.SelectUIItem(currentItemSelect, uiItem);

            currentItemSelect = uiItem;
            ResetDrag();
            itemDetailPageView.HandleItemClicked(uiItem.inventoryItem);
        }
        private void HandleItemRightClick(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItemsAlreadyOwned.IndexOf(uiItem);
            if (index < 0) return;

            OnItemActionRequested?.Invoke(index);
        }

        private void HandleBeginDrag(UIItemSlotBase uiItem)
        {
            isDraging = true;
            int index = view.listOfUIItemsAlreadyOwned.IndexOf(uiItem);
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
            int dropIndex = view.listOfUIItemsAlreadyOwned.IndexOf(uiItem);

            if (dropIndex == -1) return;

            SwapItemsUI(currentlyDraggedItemIndex, dropIndex);
            ItemClicked(uiItem);
        }

        private void SwapItemsUI(int from, int to)
        {
            var fromSlot = view.listOfUIItemsAlreadyOwned[from];
            var toSlot = view.listOfUIItemsAlreadyOwned[to];
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
            view = GetComponent<ChooseHeroPageView>();
        }
    }
}
