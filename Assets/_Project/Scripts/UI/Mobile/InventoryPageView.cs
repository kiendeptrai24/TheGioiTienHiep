using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class InventoryPageView : MonoBehaviour
    {
        [Header("UI References")]
        public Button refreshBtn;
        public RectTransform contentPanel;
        public UIInventoryItem itemPrefab;
        public MouseFollower mouseFollower;
        public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();
        public event Action<bool> OnDescriptionToggle;
        public event Action OnRefreshClicked;

        private void Awake()
        {
            refreshBtn.onClick.AddListener(() => OnRefreshClicked?.Invoke());
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
                UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
                listOfUIItems.Add(uiItem);
            }

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
