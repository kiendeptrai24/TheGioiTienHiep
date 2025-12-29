

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
        public void RefreshInventory(List<InventoryItem> listItemDatas)
        {
            #region Sort
                
            // // 1️⃣ Lấy danh sách item đang equip
            // HashSet<InventoryItem> equippedItems = new HashSet<InventoryItem>();

            // foreach (var slot in listOfEquitmentItems)
            // {
            //     if (slot.inventoryItem != null)
            //         equippedItems.Add(slot.inventoryItem);
            // }

            // // 2️⃣ Tạo list hiển thị (không ảnh hưởng data gốc)
            // List<InventoryItem> displayList = new List<InventoryItem>();

            // foreach (var item in listItemDatas)
            // {
            //     if (!equippedItems.Contains(item))
            //         displayList.Add(item);
            // }

            // // 3️⃣ Cập nhật UI
            // for (int i = 0; i < listOfUIItemsInInventory.Count; i++)
            // {
            //     if (i < displayList.Count)
            //         listOfUIItemsInInventory[i].SetItem(displayList[i]);
            //     else
            //         listOfUIItemsInInventory[i].ResetData();
            // }
            #endregion

            // 1️⃣ Tập item đang equip
            HashSet<InventoryItem> equippedItems = new HashSet<InventoryItem>();

            foreach (var slot in listOfEquitmentItems)
            {
                if (slot.inventoryItem != null)
                    equippedItems.Add(slot.inventoryItem);
            }

            // 2️⃣ Duyệt theo index – GIỮ NGUYÊN VỊ TRÍ
            for (int i = 0; i < listOfUIItemsInInventory.Count; i++)
            {
                // Không có item ở index này
                if (i >= listItemDatas.Count)
                {
                    listOfUIItemsInInventory[i].ResetData();
                    continue;
                }

                InventoryItem item = listItemDatas[i];

                // Item đang equip → slot trống
                if (equippedItems.Contains(item))
                {
                    listOfUIItemsInInventory[i].ResetData();
                }
                else
                {
                    listOfUIItemsInInventory[i].SetItem(item);
                }
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