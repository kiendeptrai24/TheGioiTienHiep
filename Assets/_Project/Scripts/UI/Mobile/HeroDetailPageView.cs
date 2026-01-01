using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class HeroDetailPageView : IItemDetailPageView
    {
        [Header("Content")]
        [SerializeField] private TextMeshProUGUI itemNameTxt;
        [SerializeField] private TextMeshProUGUI realmTxt;
        [SerializeField] private TextMeshProUGUI qualityTypeTxt;
        [SerializeField] private Image itemIconImge;
        [SerializeField] private Button biographyBtn;
        [SerializeField] private Button heroStatsBtn;
        [SerializeField] private Button heroDetailBtn;

        public List<UIEquipmentSlot> uIEquipmentSlots;
        private Dictionary<EquipmentType, UIItemSlotBase> equipmentSlotsDictionary;
        public List<UIItemSlotBase> uISkillItems;
        public List<UIItemSlotBase> uITechniqueItems;

        public event Action OnBiographyClicked;
        public event Action OnHeroStatsClicked;
        public event Action OnHeroDetailClicked;

        public event Action<InventoryItem> OnItemClicked;

        private bool setup = false;
        protected override void Awake()
        {
            base.Awake();
            biographyBtn.onClick.AddListener(() => { OnBiographyClicked?.Invoke(); });
            heroStatsBtn.onClick.AddListener(() => { OnHeroStatsClicked?.Invoke(); });
            heroDetailBtn.onClick.AddListener(() => { OnHeroDetailClicked?.Invoke(); });
        }

        private void Init()
        {
            equipmentSlotsDictionary = new Dictionary<EquipmentType, UIItemSlotBase>();
            foreach (var slot in uIEquipmentSlots)
            {
                equipmentSlotsDictionary.Add(slot.equipmentType, slot);
            }
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
                SetupHeroData(heroData);
                OnItemClicked?.Invoke(inventoryItem);
            }

        }

        private void SetupHeroData(HeroData heroData)
        {
            itemNameTxt.text = heroData.itemName;
            realmTxt.text = EnumTranslator.ToVietnamese(heroData.cultivationStage);
            qualityTypeTxt.text = EnumTranslator.ToVietnamese(heroData.qualityType);
            itemIconImge.sprite = heroData.itemIcon;

            foreach (var item in uIEquipmentSlots)
            {
                item.ResetData();
            }
            for (int i = 0; i < heroData.equitmentDatas.Count; i++)
            {
                equipmentSlotsDictionary[heroData.equitmentDatas[i].equipmentType].SetItem(new InventoryItem(heroData.equitmentDatas[i]));
            }


            // Show lên uISkillItems
            for (int i = 0; i < uISkillItems.Count; i++)
            {
                if (heroData.skillDatas != null && i < heroData.skillDatas.Count)
                {
                    var item = new InventoryItem(heroData.skillDatas[i]);
                    item.data = heroData.skillDatas[i];
                    uISkillItems[i].SetItem(item);
                }
                else
                {
                    uISkillItems[i].ResetData();
                }
            }

            // Show lên uITechniqueItems
            for (int i = 0; i < uITechniqueItems.Count; i++)
            {
                if (heroData.techniqueDatas != null && i < heroData.techniqueDatas.Count)
                {
                    var item = new InventoryItem(heroData.techniqueDatas[i]);
                    item.data = heroData.techniqueDatas[i];
                    uITechniqueItems[i].SetItem(item);
                }
                else
                {
                    uITechniqueItems[i].ResetData();
                }
            }
        }
    }
}