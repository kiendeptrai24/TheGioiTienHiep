

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
namespace TGTH.Mobile
{
    public class CharacterPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private CharacterPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        [SerializeField] private IItemDetailPageView realmDetailPageView;
        private InventoryCenterManager inventoryCenterManager;
        protected override void Awake()
        {
            base.Awake();
            LoadComponent();
            Init();
            OnItemPlayerChanged(inventoryCenterManager.playerCham);
            inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;
            view.OnRealmButtonClicked += () =>
            {
                InventoryItem inventoryItem = new InventoryItem(inventoryCenterManager.playerCham);
                realmDetailPageView?.HandleItemClicked(inventoryItem);
            };
        }

        private void Init()
        {
            foreach (var uiItem in view.uIEquipmentSlots)
            {
                uiItem.OnItemClicked += HandleItemClicked;
            }
            view.Init();
        }

        private void HandleItemClicked(UIItemSlotBase uiItem)
        {
            uiItem.navigation.OnClick();
            itemDetailPageView?.HandleItemClicked(uiItem.inventoryItem);
        }
        private void OnItemPlayerChanged(ItemData data)
        {
            view.ShowData(data);
        }

        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<CharacterPageView>();
            inventoryCenterManager = InventoryCenterManager.Instance;

        }
    }
}
