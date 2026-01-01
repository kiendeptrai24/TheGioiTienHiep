using System;
using UnityEngine;

namespace TGTH.Mobile
{
    public class HeroDetailPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private HeroDetailPageView view;
        [SerializeField] private IItemDetailPageView itemDetailPageView;
        [SerializeField] private StatsManager statsManager;
        [SerializeField] private CharacterIdentity characterIdentity;
        [SerializeField] private EquipmentSystem equipmentSystem;
        [SerializeField] private SkillSystem skillSystem;
        [SerializeField] private TechniqueSystem techniqueSystem;


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
            view.OnItemClicked += SetStatManager;
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
            Debug.Log("SetStatManager");
            statsManager.ResetStats();
            var heroData = item.data as HeroData;
            if (heroData.statsCultivationPathData == null)
            {
                Debug.Log("heroData == null");
            }
            statsManager.Setup(heroData.statsCultivationPathData, heroData.statsRealmData, heroData.statsRaceData);
            characterIdentity.Setup(heroData.statsCultivationPathData, heroData.statsRealmData, heroData.statsRaceData);
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