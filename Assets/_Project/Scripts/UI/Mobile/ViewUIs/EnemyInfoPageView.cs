using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class EnemyInfoPageView : TGTHMonoBehaviour
    {
        [Header("Content")]
        [SerializeField] private Image itemIconImge;
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private Button attackbtn;
        [SerializeField] private UIItemSlotBase avatarPrefab;
        [SerializeField] private Transform avatarContent;
        public List<UIEquipmentSlot> uIEquipmentSlots;
        public Dictionary<EquipmentType, UIItemSlotBase> equipmentSlotsDictionary;
        public List<UIItemSlotBase> uISkillItems;
        public List<UIItemSlotBase> uITechniqueItems;
        public List<UIItemSlotBase> uichamItems;

        public event Action OnAttackClicked;

        protected override void Awake()
        {
            base.Awake();
            attackbtn.onClick.AddListener(() => { OnAttackClicked?.Invoke(); });
        }
        public void ShowAllChampion(List<ItemData> itemDatas)
        {
            foreach (var item in uichamItems)
            {
                Destroy(item.gameObject);
            }
            uichamItems.Clear();

            foreach (var item in itemDatas)
            {
                var uiItem = Instantiate(avatarPrefab, avatarContent);
                uiItem.SetItem(new InventoryItem(item));
                uichamItems.Add(uiItem);
            }
        }
        public void ShowData(HeroData heroData)
        {
            if (heroData == null)
            {
                return;
            }
            nameTxt.text = heroData.itemName;
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