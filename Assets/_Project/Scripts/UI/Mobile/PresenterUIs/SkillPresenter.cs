

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
namespace TGTH.Mobile
{
    public class SkillPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private SkillPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        private List<InventoryItem> listItemDatas;
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;
        private bool isSWapped = false;
        [SerializeField] private StatsData stats;
        private bool isOwnPage = false;
        private bool isNew = true;
        private bool isShowEquipment = false;
        private HeroData heroData;
        private InventoryCenterManager inventoryCenterManager;

        protected override void Awake()
        {
            view.ToggleMouseFollower(false);
            InitializeInventoryUI(50);

            inventoryCenterManager = InventoryCenterManager.Instance;
            inventoryCenterManager.OnItemUsedDataChanged += OnListItemDataChanged;
            inventoryCenterManager.OnLoadDataSuccessed += () =>
            {
                view.Reset();
                OnListItemDataChanged(inventoryCenterManager.GetDatasUsed());
            };
            OnListItemDataChanged(inventoryCenterManager.GetDatasUsed());
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
            OnPlayerChamChanged(stats.chamionData);
            isNew = false;
            inventoryCenterManager.OnItemPlayerChanged += OnPlayerChamChanged;
        }
        private List<ItemData> ListSkillData(List<ItemData> temps)
        {
            List<ItemData> temp = new();
            foreach (var item in temps)
            {
                if (item is SkillData)
                    temp.Add(item);
            }
            return temp;
        }
        private void OnListItemDataChanged(List<ItemData> itemDatas)
        {
            if (itemDatas == null) return;
            var temp = new List<InventoryItem>();
            foreach (var item in ListSkillData(itemDatas))
            {
                temp.Add(new InventoryItem(item));
            }
            listItemDatas = temp;
            if (isOwnPage) return;
            ShowItemsInInventory();
        }

        private void OnPlayerChamChanged(ItemData data)
        {
            if (isOwnPage && isNew == false) return;
            if (data == null) return;

            heroData = data as HeroData;
            var skills = heroData.skillDatas;
            var listItems = new List<InventoryItem>();

            foreach (var item in skills)
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
                view.listOfEquitmentItems[i].OnItemClicked += HandleItemClicked;
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
                var result = inventoryCenterManager.AddUsedData(item);
                if (result)
                {
                    var skillData = item1.data as SkillData;
                    heroData.skillDatas.Remove(skillData);
                    heroData.skillIds.Remove(skillData.instanceId);
                    inventoryCenterManager.PlayerDataChanged(heroData);
                }

            }
            if (item2 != null && item2.data != null)
            {
                var result = inventoryCenterManager.RemoveUsedData(item2.data);
                if (result)
                {
                    var skillData = item2.data as SkillData;
                    heroData.skillDatas.Add(skillData);
                    heroData.skillIds.Add(skillData.instanceId);
                    inventoryCenterManager.PlayerDataChanged(heroData);
                }
            }
            if (item1 != null && item1.data != null)
            {
                item1.data = item;
            }

        }
        public void Refesh()
        {
            view.RefreshInventory(listItemDatas);
        }

        private void ShowItemsInInventory()
        {
            view.ShowItemsInInventory(listItemDatas);
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
            view.SetFollowerData(item.data.itemIcon, item.data.currentStack);

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
    }
}