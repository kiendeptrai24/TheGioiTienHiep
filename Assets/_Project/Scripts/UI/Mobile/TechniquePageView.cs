

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class TechniquePageView : TGTHMonoBehaviour
    {
        [Header("UI References")]
        //public Button refreshBtn;
        public RectTransform contentPanel;
        public UIItemSlotBase itemPrefab;
        public MouseFollower mouseFollower;
        public List<UITechniqueItem> listOfEquitmentItems = new List<UITechniqueItem>();

        public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();
        public event Action<bool> OnDescriptionToggle;
        public event Action OnRefreshClicked;

        protected override void Awake()
        {
            //refreshBtn.onClick.AddListener(() => OnRefreshClicked?.Invoke());
        }
        public void ToggleMouseFollower(bool enable)
        {
            mouseFollower.Toggle(enable);
        }

        public void SetFollowerData(Sprite sprite, int quantity)
        {
            mouseFollower.SetData(sprite, quantity);
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

        public void CreateInventorySlots(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                UIItemSlotBase uiItem = Instantiate(itemPrefab, contentPanel);
                listOfUIItems.Add(uiItem);
            }
            listOfUIItems.AddRange(listOfEquitmentItems);
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
            if (listOfUIItems.Count < listItemDatas.Count) return;
            for (int i = 0; i < listItemDatas.Count; i++)
            {
                if (i >= 50) return;
                listOfUIItems[i].SetItem(listItemDatas[i]);
            }
        }
        public void SetItem(int index, InventoryItem item)
        {
            listOfUIItems[index].SetItem(item);
        }

        public void SetItemData(int index, Sprite sprite, int qty, string name)
        {
            listOfUIItems[index].SetData(sprite, qty);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}