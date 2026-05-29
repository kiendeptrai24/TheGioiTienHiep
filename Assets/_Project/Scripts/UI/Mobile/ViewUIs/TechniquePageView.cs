

using System;
using System.Collections.Generic;
using UnityEngine;
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
        public List<UIItemSlotBase> listOfUIItemsInInventory = new List<UIItemSlotBase>();
        public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();
        public void ToggleMouseFollower(bool enable)
        {
            mouseFollower.Toggle(enable);
        }

        public void SetFollowerData(Sprite sprite, int quantity)
        {
            mouseFollower.SetData(sprite, quantity);
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
                listOfUIItemsInInventory.Add(uiItem);

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

        public void ShowItemsInventory(List<InventoryItem> listItemDatas)
        {
            if (listItemDatas == null) return;
            for (int i = 0; i < listOfUIItemsInInventory.Count; i++)
            {
                if (i >= 50) return;
                if (i >= listItemDatas.Count)
                {
                    listOfUIItemsInInventory[i].ResetData();
                }
                else
                {
                    listOfUIItemsInInventory[i].SetItem(listItemDatas[i]);
                }
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        internal void ShowItemEquipment(List<InventoryItem> listItems)
        {
            if (listItems == null) return;
            int itemIndex = 0;
            for (int i = 0; i < listOfEquitmentItems.Count; i++)
            {
                if (itemIndex >= listItems.Count)
                {
                    listOfEquitmentItems[i].ResetData();
                    continue;
                }
                if (listOfEquitmentItems[i].IsLocked()) continue;
                listOfEquitmentItems[i].SetItem(listItems[itemIndex]);
                itemIndex++;
            }
        }

        public void Reset()
        {
            foreach (var item in listOfUIItems)
                item.ResetData();
            foreach (var item in listOfEquitmentItems)
                item.ResetData();
        }
    }
}