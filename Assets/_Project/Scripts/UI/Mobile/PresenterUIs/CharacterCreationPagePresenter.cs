


using System;
using System.Collections.Generic;
using TGTH.Mobile;
using UnityEngine;
namespace TGTH.Mobile
{
    public class CharacterCreationPagePresenter : IItemClickHandler
    {
        [SerializeField] private CharacterCreationPageView view;
        [SerializeField] private IItemDetailPageView itemOnClick;
        [SerializeField] private ActionNavigation navigation;
        [SerializeField] private UIItemSlotBase currentItemSelect;
        [SerializeField] private UIItemSlotBase currentItemCharacter;
        [SerializeField] private string nameCharacter = "";
        [SerializeField] private List<ItemPreset> allCharacter;
        private List<ItemData> itemDatas = new List<ItemData>();
        protected override void Awake()
        {
            base.Awake();
            LoadComponent();
            view.OnStartClicked += OnStartClicked;
            view.OnFieldEndEdit += OnFieldEndEdit;
            Init();
        }

        private void OnFieldEndEdit(string obj)
        {
            nameCharacter = obj;
        }

        private void OnStartClicked()
        {
            if (currentItemCharacter == null || currentItemSelect == null || nameCharacter == "") return;
            var itemData = currentItemSelect.inventoryItem.data as HeroData;
            itemData.itemName = nameCharacter;
            InventoryItem inventoryItem = new InventoryItem(itemData);

            itemOnClick.HandleItemClicked(inventoryItem);
            navigation.OnClick();
        }

        protected override void Start()
        {
            base.Start();
            foreach (var item in allCharacter)
            {
                itemDatas.Add(item.GetItemData());
            }
            ShowItem(itemDatas);

        }
        private void ShowItem(List<ItemData> listItem)
        {
            var itemInventories = new List<InventoryItem>();
            foreach (var item in listItem)
            {
                itemInventories.Add(new InventoryItem(item));
            }
            view.ShowAllItems(itemInventories);
        }
        private void Init()
        {
            foreach (var uiItem in view.listOfUIItems)
            {
                uiItem.OnItemClicked += HandleItemClicked;
            }
        }

        private void HandleItemClicked(UIItemSlotBase uiItem)
        {
            if (uiItem == null) return;
            view.ShowInfo(uiItem);
            ItemClicked(uiItem);
        }
        private void ItemClicked(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;

            view.SelectUIItem(currentItemSelect, uiItem);

            currentItemSelect = uiItem;
        }

        public override void OnItemClicked(UIItemSlotBase uiItem)
        {
            currentItemCharacter = uiItem;
        }
    }
}