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
        [SerializeField] private CharacterIdentity characterIdentity;
        [SerializeField] private EquipmentSystem equipmentSystem;
        [SerializeField] private SkillSystem skillSystem;
        [SerializeField] private TechniqueSystem techniqueSystem;

        private bool setup = false;
        protected override void Awake()
        {
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
        private void OnEnable()
        {
            ShowData(new InventoryItem(statsManager.heroData));
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
            Debug.Log("Show Data Hero Detail Page Presenter");
            SetStatManager(inventoryItem);
            Debug.Log((inventoryItem.data as HeroData).equitmentDatas.Count);
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
            statsManager.SetupData(heroData.statsCultivationPathData, heroData.statsRealmData, heroData.statsRaceData);
            characterIdentity.SetupData(heroData.statsCultivationPathData, heroData.statsRealmData, heroData.statsRaceData);
            foreach (var eq in heroData.equitmentDatas)
            {
                equipmentSystem.Equip(new InventoryItem(eq));
            }
            foreach (var skill in heroData.skillDatas)
            {
                skillSystem.Equip(new InventoryItem(skill));
            }
            foreach (var technique in heroData.techniqueDatas)
            {
                techniqueSystem.Equip(new InventoryItem(technique));
            }
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