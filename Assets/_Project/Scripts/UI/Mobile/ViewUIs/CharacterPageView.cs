using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class CharacterPageView : TGTHMonoBehaviour
    {
        [Header("Content")]
        [SerializeField] private TextMeshProUGUI itemNameTxt;
        [SerializeField] private TextMeshProUGUI realmTxt;
        [SerializeField] private Image itemIconImge;
        [SerializeField] private Button realmBtn;
        [SerializeField] private TextMeshProUGUI realmBtnTxt;
        public List<UIEquipmentSlot> uIEquipmentSlots;
        public Dictionary<EquipmentType, UIItemSlotBase> equipmentSlotsDictionary = new();

        public event Action OnBiographyClicked;
        public event Action OnHeroStatsClicked;
        public event Action OnHeroDetailClicked;
        public event Action OnRealmButtonClicked;



        protected override void Awake()
        {
            base.Awake();
            realmBtn.onClick.AddListener(() => OnRealmButtonClicked?.Invoke());
        }

        public void SetRealmBtnName(string name)
        {
            realmBtnTxt.text = name;
        }
        public void Init()
        {
            foreach (var item in uIEquipmentSlots)
            {
                equipmentSlotsDictionary.Add(item.equipmentType, item);
            }
        }

        public void ShowData(ItemData itemData)
        {
            var heroData = itemData as HeroData;
            if (heroData == null)
            {
                Debug.LogWarning("ShowData failed: itemData is not HeroData");
                return;
            }
            ShowEquipmentItems(heroData);
            itemNameTxt.text = heroData.itemName;
            realmTxt.text = EnumTranslator.ToVietnamese(heroData.realmType);
            itemIconImge.sprite = heroData.itemIcon;
        }
        private void ShowEquipmentItems(HeroData heroData)
        {
            if (heroData == null || heroData.equipmentDatas == null)
                return;
            RefreshEquipmentItems();
            var equipmentDatas = heroData.equipmentDatas;

            for (int i = 0; i < equipmentDatas.Count; i++)
            {
                var item = equipmentSlotsDictionary[equipmentDatas[i].equipmentType];
                item.SetItem(new InventoryItem(equipmentDatas[i]));
            }
        }
        public void RefreshEquipmentItems()
        {
            foreach (var slot in equipmentSlotsDictionary.Values)
            {
                slot.ResetData();
            }
        }

    }
}