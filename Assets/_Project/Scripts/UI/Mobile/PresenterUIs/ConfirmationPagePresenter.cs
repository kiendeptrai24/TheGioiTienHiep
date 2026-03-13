


using System;
using System.Collections.Generic;
using TGTH.Mobile;
using UnityEngine;
namespace TGTH.Mobile
{
    public class ConfirmationPagePresenter : IItemDetailPageView
    {
        [SerializeField] private ConfirmationPageView view;
        private InventoryCenterManager inventoryCenterManager;

        private ItemData itemData;
        protected override void Awake()
        {
            base.Awake();
            LoadComponent();
            inventoryCenterManager = InventoryCenterManager.Instance;
            view.OnOkClicked += HandleOkClicked;
            view.OnExitClicked += HandleExitClicked;
        }

        private void HandleExitClicked()
        {

        }

        private void HandleOkClicked()
        {
            if (itemData == null) return;

            inventoryCenterManager.AddCharacter(itemData.Clone());
        }
        public override void HandleItemClicked(InventoryItem inventoryItem)
        {
            if (inventoryItem == null) return;
            itemData = inventoryItem.data;
        }
    }
}