using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class InventoryPageView : MonoBehaviour
    {
        public enum InventoryItemType
        {
            item,
            Equipment,
            Orther
        }
        [Header("UI References")]
        public Button sortBtn;
        public Button showAllItemsBtn;
        public TMP_Dropdown itemtypeDrop;
        public TMP_Dropdown qualityTypeDrop;
        public TextMeshProUGUI priceText;

        public RectTransform contentPanel;
        public UIInventoryItem itemPrefab;
        public MouseFollower mouseFollower;
        public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();
        public event Action OnSortClicked;
        public event Action OnRefreshClicked;
        private void Awake()
        {
            showAllItemsBtn.onClick.AddListener(() => OnRefreshClicked?.Invoke());
            sortBtn.onClick.AddListener(() => OnSortClicked?.Invoke());
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
            ClearAllSlots();
            for (int i = 0; i < listItemDatas.Count; i++)
            {
                listOfUIItems[i].SetItem(listItemDatas[i]);
            }
        }
        public void RefreshInventory(List<InventoryItem> listItemDatas)
        {
            ShowAllItems(listItemDatas);
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
