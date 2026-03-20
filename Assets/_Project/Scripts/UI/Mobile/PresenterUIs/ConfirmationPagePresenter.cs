


using System;
using System.Collections.Generic;
using TGTH.Mobile;
using UnityEngine;
namespace TGTH.Mobile
{
    public class ConfirmationPagePresenter : IItemDetailPageView
    {
        [SerializeField] private ConfirmationPageView view;
        private PlayfabDataManager playfabDataManager;
        private ItemData itemData;
        protected override void Awake()
        {
            base.Awake();
            LoadComponent();
            playfabDataManager = PlayfabDataManager.Instance;
            view.OnOkClicked += HandleOkClicked;
            view.OnExitClicked += HandleExitClicked;
        }

        private void HandleExitClicked()
        {

        }

        private void HandleOkClicked()
        {
            if (itemData == null) return;
            playfabDataManager.AddCharacter(itemData);
        }
        public override void HandleItemClicked(InventoryItem inventoryItem)
        {
            if (inventoryItem == null) return;
            itemData = inventoryItem.data;
        }
    }
}