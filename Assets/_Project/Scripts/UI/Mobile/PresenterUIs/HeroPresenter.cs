using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    public class HeroPresenter : TGTHMonoBehaviour, IPointerClickHandler, IEndDragHandler
    {
        [SerializeField] private HeroPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        private InventoryCenterManager inventoryCenterManager;
        private List<InventoryItem> listItemDatas;
        [SerializeField] private List<HeroData> rootListDatas;
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;
        [SerializeField] private StatsData statsManager;

        protected override void Awake()
        {
            view.OnRefreshClicked += ShowItemsAlreadyOwned;
            view.OnSortClicked += SortInventory;

            view.ToggleMouseFollower(false);
            InitializeInventoryUI(50);
            LoadItem();
        }

        private void LoadItem()
        {
            inventoryCenterManager = InventoryCenterManager.Instance;
            inventoryCenterManager.OnItemChampionDataChanged += RefreshInventory;

            rootListDatas = GameDataCenterManager.Instance.GetChampionDatas();

            var heroDataList = inventoryCenterManager.GetDataType(ItemType.Champion).ToList();
            heroDataList.AddRange(inventoryCenterManager.GetDatasChampionInTeam());

            SetItemData(inventoryCenterManager.GetDataType(ItemType.Champion));
            SetItemDataDontHave(heroDataList);
            inventoryCenterManager.OnItemDataChanged += OnItemDataChanged;
        }

        private void OnItemDataChanged(List<ItemData> list)
        {
            var heroDataList = inventoryCenterManager.GetDataType(ItemType.Champion).ToList();
            heroDataList.AddRange(inventoryCenterManager.GetDatasChampionInTeam());
            RefreshInventory(heroDataList);
        }

        private void RefreshInventory(List<ItemData> list)
        {
            SetItemDataDontHave(list);
            SetItemData(inventoryCenterManager.GetDataType(ItemType.Champion, true));
        }

        private void SetItemDataDontHave(List<ItemData> list)
        {
            var temp = new List<InventoryItem>();

            foreach (var item in rootListDatas)
            {
                if (item is HeroData)
                {
                    if (item.isCharacter) continue;
                    if (!list.Contains(item))
                        temp.Add(new InventoryItem(item));
                }
            }
            view.ShowAllItemsNotYetOwned(temp);
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
            foreach (var uiItem in view.listOfUIItemsNotYetOwned)
            {
                uiItem.OnItemClicked += HandleItemClicked;
            }

        }

        private void SetItemData(List<ItemData> list)
        {
            var temp = new List<InventoryItem>();
            foreach (var item in list)
            {
                temp.Add(new InventoryItem(item));
            }
            listItemDatas = temp;
            view.ShowAllItemsAlreadyOwned(listItemDatas);
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
            view.ShowAllItemsAlreadyOwned(sortedList);
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
            statsManager.SetUpItem(uiItem.inventoryItem.data);
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

        private void ShowItemsAlreadyOwned()
        {
            view.ShowAllItemsAlreadyOwned(listItemDatas);
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
            view = GetComponent<HeroPageView>();
        }
    }
}
