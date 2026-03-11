

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class SkillPageView : TGTHMonoBehaviour
    {
        [Header("UI References")]
        //public Button refreshBtn;
        public RectTransform contentPanel;
        public UIItemSlotBase itemPrefab;
        public MouseFollower mouseFollower;
        public List<UISkillItem> listOfEquitmentItems = new List<UISkillItem>();
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
        public void ShowItemsInInventory(List<InventoryItem> listItemDatas)
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
        public void ShowItemEquipment(List<InventoryItem> listItemDatas)
        {
            if (listItemDatas == null) return;
            for (int i = 0; i < listOfEquitmentItems.Count; i++)
            {
                if (listOfEquitmentItems[i].IsLocked()) continue;
                if (i >= listItemDatas.Count)
                {
                    listOfEquitmentItems[i].ResetData();
                }
                else
                {
                    listOfEquitmentItems[i].SetItem(listItemDatas[i]);
                }
            }
        }
        public void RefreshInventory(List<InventoryItem> listItemDatas)
        {
            if (listItemDatas == null) return;

            for (int i = 0; i < listOfUIItemsInInventory.Count; i++)
            {
                if (i >= listItemDatas.Count)
                {
                    listOfUIItemsInInventory[i].ResetData();
                    continue;
                }

                InventoryItem item = listItemDatas[i];
                listOfUIItemsInInventory[i].SetItem(item);
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