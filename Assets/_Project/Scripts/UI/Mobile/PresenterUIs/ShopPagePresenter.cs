using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Globalization;
using System.Text;
using Unity.Netcode;

namespace TGTH.Mobile
{
    public class ShopPagePresenter : TGTHMonoBehaviour, IPointerClickHandler, IEndDragHandler
    {
        [SerializeField] private ShopPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        [SerializeField] private ShopUseSystem shopUseSystem;
        private List<InventoryItem> listItemDatas = new List<InventoryItem>();
        private UIItemSlotBase currentItemSelect;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> OnItemActionRequested;
        public event Action<int> OnStartDragging;
        private bool isDraging = false;
        private GameDataCenterManager gameDCM;
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        protected override void Awake()
        {
            base.Awake();
            gameDCM = GameDataCenterManager.Instance;

            ProfileManager.Instance.OnProfileChanged += (profile) =>
            {
                view.priceText.text = profile.coins.ToString();
            };
            view.priceText.text = ProfileManager.Instance.GetProfile().coins.ToString();
            view.OnRefreshClicked += ShowItem;
            view.OnEquipmentTypeChanged += SortInventoryEquipmentType;
            view.OnTechniqueAndSkillTypeChanged += SortInventoryType;
            view.OnOtherTypeChanged += SortInventoryOtherType;
            view.OnSearchItemSubmit += SearchItemInventory;
            view.ToggleMouseFollower(false);

            var itemdatas = gameDCM.GetShopDatas();

            InitializeInventoryUI(itemdatas.Count);
            SetInventoryData(itemdatas);
        }

        private void SearchItemInventory(string value)
        {
            if (listItemDatas == null || listItemDatas.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(value))
            {
                view.ShowAllItems(listItemDatas);
                return;
            }

            string keyword = RemoveDiacritics(value)
                .ToLowerInvariant()
                .Trim();

            var result = listItemDatas
                .Where(item =>
                    item.data.itemName != null &&
                    RemoveDiacritics(item.data.itemName)
                        .ToLowerInvariant()
                        .Contains(keyword))
                .ToList();

            view.ShowAllItems(result);
        }

        private void InitializeInventoryUI(int amount)
        {
            view.CreateInventorySlots(amount);

            foreach (var uiItem in view.listOfUIItems)
            {
                uiItem.OnItemClicked += HandleItemClicked;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleItemRightClick;
            }
        }
        public void Refesh()
        {
            view.RefreshInventory(listItemDatas);
        }

        public void SetInventoryData(List<ItemData> items)
        {
            listItemDatas.Clear();
            foreach (var item in items)
            {
                listItemDatas.Add(new InventoryItem(item));
            }
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
        public void SortInventoryEquipmentType(int value)
        {
            if (listItemDatas == null || listItemDatas.Count == 0) return;
            int type = value;
            EquipmentType selectedType = (EquipmentType)type + 1;

            var sortedList = listItemDatas
                .Where(item =>
                    item.data is EquipmentData equipment &&
                    equipment.equipmentType == selectedType)
                .OrderBy(item => item.data.itemType)
                .ThenByDescending(item => item.data.qualityType)
                .ToList();

            view.ShowAllItems(sortedList);
        }
        public void SortInventoryType(int value)
        {
            if (listItemDatas == null || listItemDatas.Count == 0) return;
            ItemType typeIndex = (ItemType)value;
            List<InventoryItem> sortedList;
            if (typeIndex == ItemType.Material)
            {
                sortedList = listItemDatas
                    .Where(item =>
                        item.data.itemType != ItemType.Champion)
                    .OrderBy(item => item.data.itemType)
                    .ThenByDescending(item => item.data.qualityType)
                    .ToList();
            }
            else
            {
                sortedList = listItemDatas
                .Where(item =>
                    item.data.itemType == typeIndex)
                .OrderBy(item => item.data.itemType)
                .ThenByDescending(item => item.data.qualityType)
                .ToList();
            }


            view.ShowAllItems(sortedList);
        }
        public void SortInventoryOtherType(int value)
        {

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
            var popup = PopupManager.Instance.GetPopup<BuyItemPopup>();
            ShopDataPopup shopData = new ShopDataPopup();
            shopData.title = uiItem.inventoryItem.data.itemName;
            shopData.itemIcon = uiItem.inventoryItem.data.itemIcon;
            shopData.type = uiItem.inventoryItem.data.itemType;
            shopData.realm = uiItem.inventoryItem.data.realmType;
            shopData.quanlity = uiItem.inventoryItem.data.qualityType;
            shopData.price = uiItem.inventoryItem.data.itemPrice;

            ShopSetupData data = new ShopSetupData(shopData);

            if (popup != null)
            {
                popup.ShowPopup(data,
                onConfirm: (QuantityPopupData result) =>
                {
                    ulong price = uiItem.inventoryItem.data.itemPrice * (ulong)result.quantity;
                    var item = uiItem.inventoryItem.data;
                    ShopRequester.Instance.RequestBuy(item.instanceId, (success, message) =>
                    {
                        if (success)
                        {
                            var playerClientId = NetworkManager.Singleton.LocalClientId;
                            shopUseSystem.UseItem(playerClientId, uiItem, result.quantity);
                            TopNotificationUI.Instance.ShowNotification(message);
                        }
                        else
                        {
                            TopNotificationUI.Instance.ShowNotification(message);
                        }
                    });
                },
                onCancel: () =>
                {

                },
                onShowInfo: () =>
                {
                    if (uiItem == null)
                    {
                        Debug.Log("uiItem is null");
                    }
                    uiItem?.navigation.OnClick();
                });

            }
        }

        private void ItemClicked(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;

            view.DeselectItem(currentItemSelect);
            view.SelectUIItem(currentItemSelect, uiItem);

            currentItemSelect = uiItem;
            ResetDrag();
            itemDetailPageView?.HandleItemClicked(uiItem.inventoryItem);
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
            view = GetComponent<ShopPageView>();
            shopUseSystem = GetComponent<ShopUseSystem>();
        }
    }
}
