using System;
using System.Collections.Generic;
using DuloGames.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class CharacterCreationPageView : TGTHMonoBehaviour
    {
        [SerializeField] private Button startBtn;
        [SerializeField] private TMP_InputField nameNvField;
        [SerializeField] private TextMeshProUGUI descriptionTxt;
        [SerializeField] private Image itemIconImge;
        public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();
        public event Action OnStartClicked;
        public event Action<string> OnFieldEndEdit;
        protected override void Awake()
        {
            base.Awake();
            startBtn.onClick.AddListener(() => OnStartClicked?.Invoke());
            nameNvField.onEndEdit.AddListener((value) => OnFieldEndEdit?.Invoke(value));
        }
        public void ShowInfo(UIItemSlotBase uiItem)
        {
            if (uiItem == null) return;
            if (uiItem.inventoryItem == null) return;
            descriptionTxt.text = uiItem.inventoryItem.data.itemDescription;
            itemIconImge.sprite = uiItem.inventoryItem.data.itemIcon;
        }
        public void ClearAllSlots()
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
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
                if (i >= listItemDatas.Count) return;
                listOfUIItems[i].SetItem(listItemDatas[i]);
            }
        }
    }
}