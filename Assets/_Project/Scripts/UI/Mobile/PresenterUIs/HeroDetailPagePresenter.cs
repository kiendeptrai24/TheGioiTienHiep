using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace TGTH.Mobile
{
    public class HeroDetailPagePresenter : IItemDetailPageView
    {
        [SerializeField] private HeroDetailPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        [SerializeField] private StatsData statsManager;
        private InventoryCenterManager inventoryCenterManager;
        private bool setup = false;
        protected override void Awake()
        {
            inventoryCenterManager = InventoryCenterManager.Instance;
            inventoryCenterManager.OnItemExistingEquitmentDataChanged += SetItemData;
            foreach (var uiItem in view.uIEquipmentSlots)
            {
                uiItem.OnItemClicked += HandleItemClicked;
            }
            foreach (var uiItem in view.uISkillItems)
            {
                uiItem.OnItemClicked += HandleItemClicked;
            }
            foreach (var uiItem in view.uITechniqueItems)
            {
                uiItem.OnItemClicked += HandleItemClicked;
            }
            view.OnBiographyClicked += ShowBiography;
            view.OnHeroStatsClicked += ShowHeroInfo;
            view.OnHeroDetailClicked += ShowHeroDetail;
        }

        private void SetItemData(List<ItemData> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item.itemId == statsManager.heroData.itemId)
                {
                    ShowData(new InventoryItem(item));
                    break;
                }
            }
            foreach (var uiItem in view.uIEquipmentSlots)
            {
                uiItem.ResetData();
            }
            var heroData = statsManager.heroData as HeroData;
            foreach (var item in heroData.equipmentDatas)
            {
                view.equipmentSlotsDictionary[item.equipmentType].SetItem(new InventoryItem(item));
            }
        }
        private void Init()
        {
            view.equipmentSlotsDictionary = new Dictionary<EquipmentType, UIItemSlotBase>();
            foreach (var slot in view.uIEquipmentSlots)
            {
                view.equipmentSlotsDictionary.Add(slot.equipmentType, slot);
            }
        }
        public void ShowData(InventoryItem inventoryItem)
        {
            SetStatManager(inventoryItem);
            view.ShowData(inventoryItem.data as HeroData);
        }
        public override void HandleItemClicked(InventoryItem inventoryItem)
        {
            if (!setup)
            {
                Init();
                setup = true;
            }

            if (inventoryItem == null) return;
            if (inventoryItem.data is HeroData heroData)
            {
                view.ShowData(heroData);
                SetStatManager(inventoryItem);
            }

        }

        private void ShowBiography()
        {

        }

        private void ShowHeroInfo()
        {

        }
        private void ShowHeroDetail()
        {

        }
        public void SetStatManager(InventoryItem item)
        {
            if (item == null || item.data == null) return;
            statsManager.ResetStats();
            var heroData = item.data as HeroData;
            if (heroData == null) return;
            statsManager.SetUpItem(heroData);
        }

        private void HandleItemClicked(UIItemSlotBase uiItem)
        {

            itemDetailPageView?.HandleItemClicked(uiItem.inventoryItem);
            uiItem?.navigation.OnClick();
        }

        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<HeroDetailPageView>();
        }
    }
}