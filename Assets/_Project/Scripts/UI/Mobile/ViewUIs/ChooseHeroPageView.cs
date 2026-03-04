using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class ChooseHeroPageView : MonoBehaviour
    {
        [Header("UI References")]
        public Button sortBtn;
        public Button showAllItemsBtn;
        public TMP_Dropdown itemtypeDrop;
        public TMP_Dropdown qualityTypeDrop;

        public RectTransform contentHeroExists;
        public UIInventoryItem itemPrefab;
        public MouseFollower mouseFollower;
        public List<UIItemSlotBase> listOfUIItemsAlreadyOwned = new List<UIItemSlotBase>();
        public event Action OnSortClicked;
        public event Action OnRefreshClicked;
        private InventoryItem currentItemSelect;

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
            foreach (var item in listOfUIItemsAlreadyOwned)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        public void DeselectAll()
        {
            foreach (var item in listOfUIItemsAlreadyOwned)
                item.Deselect();
        }

        public void CreateInventorySlots(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (contentHeroExists == null) break;
                UIInventoryItem uiItem = Instantiate(itemPrefab, contentHeroExists);
                listOfUIItemsAlreadyOwned.Add(uiItem);
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
            if (listOfUIItemsAlreadyOwned.Count < listItemDatas.Count) return;
            ClearAllSlots();
            for (int i = 0; i < listItemDatas.Count; i++)
            {
                listOfUIItemsAlreadyOwned[i].SetItem(listItemDatas[i]);
            }
        }

        public void SetItem(int index, InventoryItem item)
        {
            listOfUIItemsAlreadyOwned[index].SetItem(item);
        }

        public void SetItemData(int index, Sprite sprite, int qty, string name)
        {
            listOfUIItemsAlreadyOwned[index].SetData(sprite, qty);
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
