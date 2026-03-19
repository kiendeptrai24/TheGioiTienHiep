using System;
using System.Collections.Generic;
using DuloGames.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class CharacterSelectionPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Button startBtn;
        [SerializeField] private TextMeshProUGUI nameNvTxt;
        [SerializeField] private Image iconNvImge;
        public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();
        public event Action OnStartClicked;
        protected override void Awake()
        {
            base.Awake();
            startBtn.onClick.AddListener(() => OnStartClicked?.Invoke());
        }
        public void ClearAllSlots()
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }
        public void ShowData(ItemData itemData)
        {
            if (itemData == null) return;
            nameNvTxt.text = itemData.itemName;
            iconNvImge.sprite = itemData.itemIcon;
        }
        public void DeselectAll()
        {
            foreach (var item in listOfUIItems)
                item.Deselect();
        }
        public void DeselectItem(UIItemSlotBase uiItem)
        {
            if (uiItem)
            {
                uiItem.Deselect();
                uiItem = null;
            }
        }
        public void SelectUIItem(UIItemSlotBase uiItemOld, UIItemSlotBase uiItemNew)
        {
            if (uiItemOld != null)
                uiItemOld.Deselect();
            uiItemOld = uiItemNew;
            uiItemOld.Select();
        }
        public void ShowAllItems(List<InventoryItem> listItemDatas)
        {
            if (listItemDatas == null) return;
            ClearAllSlots();
            for (int i = 0; i < listOfUIItems.Count; i++)
            {
                if (i >= listItemDatas.Count)
                {
                    listOfUIItems[i].ResetData();
                    Debug.Log("ResetData");
                }
                else
                {
                    listOfUIItems[i].SetItem(listItemDatas[i]);
                }
            }
        }
    }
}