


using System;
using System.Collections.Generic;
using TGTH.Mobile;
using UnityEngine;
namespace TGTH.Mobile
{
    public class CharacterSelectionPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private CharacterSelectionPageView view;
        [SerializeField] private IItemClickHandler itemOnClick;
        [SerializeField] private ActionNavigation navigation;
        private InventoryCenterManager inventoryCenterManager;
        [SerializeField] private UIItemSlotBase currentItemSelect;

        protected override void Awake()
        {
            base.Awake();
            LoadComponent();
            Init();
            inventoryCenterManager = InventoryCenterManager.Instance;
            inventoryCenterManager.OnItemCharacterChanged += OnItemCharacterChanged;
            view.OnStartClicked += OnStartClicked;
        }

        private void OnStartClicked()
        {
            if (currentItemSelect == null || currentItemSelect.HasItem() == false) return;
            inventoryCenterManager.ItemPlayerChanged(currentItemSelect.inventoryItem.data);
            navigation.OnClick();
        }

        private void OnItemCharacterChanged(List<ItemData> list)
        {
            var temp = new List<InventoryItem>();
            foreach (var item in list)
            {
                temp.Add(new InventoryItem(item));
            }
            view.ShowAllItems(temp);
        }

        private void Init()
        {
            view.ClearAllSlots();
            foreach (var uiItem in view.listOfUIItems)
            {
                var item = uiItem as UICharacterSelection;
                if (item == null)
                    continue;
                item.OnItemClicked += HandleItemClicked;
                item.OnItemEmptySlotClicked += HandleEmptySlotClicked;
            }
        }

        private void HandleEmptySlotClicked(UIItemSlotBase uiItem)
        {
            uiItem?.navigation?.OnClick();
            itemOnClick?.OnItemClicked(uiItem);
            ItemClicked(uiItem);
        }

        private void HandleItemClicked(UIItemSlotBase uiItem)
        {
            if (uiItem == null || uiItem.inventoryItem == null) return;
            view.ShowData(uiItem.inventoryItem.data);
            ItemClicked(uiItem);
        }
        private void ItemClicked(UIItemSlotBase uiItem)
        {
            int index = view.listOfUIItems.IndexOf(uiItem);
            if (index < 0) return;

            view.DeselectItem(currentItemSelect);
            view.SelectUIItem(currentItemSelect, uiItem);

            currentItemSelect = uiItem;
        }
    }
}