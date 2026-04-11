

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
        private InventoryCenterManager inventoryCenterManager;
        private List<InventoryItem> listItemDatas = new List<InventoryItem>();
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;
        private bool isSWapped = false;
        private bool isOwnPage = false;
        private bool isShowEquipment = false;
        private bool isNew = true;
        private HeroData heroData;
        [SerializeField] private StatsData stats;
        protected override void Awake()
        {
            view.ToggleMouseFollower(false);
            InitializeInventoryUI(50);

            inventoryCenterManager = InventoryCenterManager.Instance;
            inventoryCenterManager.OnItemExistingTechniqueDataChanged += SetListDataChanged;
            SetListDataChanged(inventoryCenterManager.GetDataType(ItemType.Technique, true));
        }
        private void OnEnable()
        {
            isOwnPage = true;
        }
        private void OnDisable()
        {
            isOwnPage = false;
        }
        protected override void Start()
        {
            base.Start();
            OnPlayerChamChanged(stats.heroData);
            isNew = false;
            inventoryCenterManager.OnItemPlayerChanged += OnPlayerChamChanged;
        }

        private void OnPlayerChamChanged(ItemData heroData)
        {
            if (isOwnPage && isNew == false) return;
            if (heroData == null) return;
            this.heroData = heroData as HeroData;
            var techniques = this.heroData.techniqueDatas;
            var listItems = new List<InventoryItem>();

            foreach (var item in techniques)
            {
                listItems.Add(new InventoryItem(item));
            }
            ShowItemEquipment(listItems);
        }
        private void ShowItemEquipment(List<InventoryItem> listItemDatas)
        {
            isShowEquipment = true;
            view.ShowItemEquipment(listItemDatas);
            isShowEquipment = false;
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
        private void SetListDataChanged(List<ItemData> items)
        {
            if (listItemDatas == null)
                listItemDatas = new List<InventoryItem>();
            else
                listItemDatas.Clear();
            foreach (var item in items)
            {
                listItemDatas.Add(new InventoryItem(item));
            }
            if (isOwnPage) return;
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
        private void HandleEquippedChanged(InventoryItem item1, InventoryItem item2)
        {
            if (isShowEquipment) return;
            ItemData item = null;
            if (item1 != null && item1.data != null)
            {
                if (item1.data.canStack == false)
                {
                    item = item1.data.Clone();
                }
                var result = inventoryCenterManager.AddData(item);
                if (result)
                {
                    var techniqueData = item1.data as TechniqueData;
                    heroData.techniqueDatas.Remove(techniqueData);
                    inventoryCenterManager.ItemPlayerChanged(heroData);
                }
            }
            if (item2 != null && item2.data != null)
            {
                var result = inventoryCenterManager.RemoveData(item2.data);
                if (result)
                {
                    var techniqueData = item2.data as TechniqueData;
                    heroData.techniqueDatas.Add(techniqueData);
                    inventoryCenterManager.ItemPlayerChanged(heroData);
                }
            }
            if (item1 != null && item1.data != null)
            {
                item1.data = item;
            }
        }

        public void ShowAllItems()
        {
            view.ShowItemsInventory(listItemDatas);
        }

        private void HandleItemClicked(UIItemSlotBase uiItem)
        {
            if (isDraging)
            {
                isDraging = false;
                return;
            }
            ItemClicked(uiItem);
            uiItem?.navigation?.OnClick();
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