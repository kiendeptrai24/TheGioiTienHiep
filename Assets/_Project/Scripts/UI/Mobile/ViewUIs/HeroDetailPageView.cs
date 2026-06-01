using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class HeroDetailPageView : TGTHMonoBehaviour
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
        public Dictionary<EquipmentType, UIItemSlotBase> equipmentSlotsDictionary;
        public List<UIItemSlotBase> uISkillItems;
        public List<UIItemSlotBase> uITechniqueItems;

        public event Action OnBiographyClicked;
        public event Action OnHeroStatsClicked;
        public event Action OnHeroDetailClicked;
        

        protected override void Awake()
        {
            base.Awake();
            biographyBtn.onClick.AddListener(() => { OnBiographyClicked?.Invoke(); });
            heroStatsBtn.onClick.AddListener(() => { OnHeroStatsClicked?.Invoke(); });
            heroDetailBtn.onClick.AddListener(() => { OnHeroDetailClicked?.Invoke(); });
        }

        public void ShowData(HeroData heroData)
        {
            if (heroData == null)
            {
                return;
            }
            itemNameTxt.text = heroData.itemName;
            realmTxt.text = EnumTranslator.ToVietnamese(heroData.realmType);
            qualityTypeTxt.text = EnumTranslator.ToVietnamese(heroData.qualityType);
            itemIconImge.sprite = heroData.itemIcon;

            foreach (var item in uIEquipmentSlots)
            {
                item.ResetData();
            }
            for (int i = 0; i < heroData.equipmentDatas.Count; i++)
            {
                equipmentSlotsDictionary[heroData.equipmentDatas[i].equipmentType].SetItem(new InventoryItem(heroData.equipmentDatas[i]));
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