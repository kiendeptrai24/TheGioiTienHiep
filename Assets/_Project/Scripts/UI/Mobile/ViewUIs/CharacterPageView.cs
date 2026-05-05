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
            if(heroData == null)
            {
                Debug.LogError("ShowData failed: itemData is not HeroData");
                return;
            }
            if(heroData.equipmentDatas == null)
            {
                Debug.LogError("ShowData failed: equipmentDatas is null");
                return;
            }
            var equipmentDatas = heroData.equipmentDatas;

            for (int i = 0; i < equipmentDatas.Count; i++)
            {
                var item = equipmentSlotsDictionary[equipmentDatas[i].equipmentType];
                item.SetItem(new InventoryItem(equipmentDatas[i]));
            }
            itemNameTxt.text = heroData.itemName;
            realmTxt.text = EnumTranslator.ToVietnamese(heroData.realmType);
            itemIconImge.sprite = heroData.itemIcon;
        }
    }
}