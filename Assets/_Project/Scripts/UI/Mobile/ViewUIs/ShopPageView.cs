using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGTH.Mobile
{
    public class ShopPageView : MonoBehaviour
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
        public TextMeshProUGUI priceText;
        public TMP_Dropdown equipmentTypeDrop;
        public TMP_Dropdown techniqueAndSkillTypeDrop;
        public TMP_Dropdown otherDrop;
        public TMP_InputField searchItemField;

        public RectTransform contentPanel;
        public UIItemSlotBase itemPrefab;
        public MouseFollower mouseFollower;
        public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();


        public event Action OnSortClicked;
        public event Action OnRefreshClicked;
        public event Action<int> OnEquipmentTypeChanged;
        public event Action<int> OnTechniqueAndSkillTypeChanged;
        public Action<int> OnOtherTypeChanged;
        public Action<string> OnSearchItemSubmit;

        private void Awake()
        {
            showAllItemsBtn.onClick.AddListener(() => OnRefreshClicked?.Invoke());
            sortBtn.onClick.AddListener(() => OnRefreshClicked?.Invoke());
            sortBtn.onClick.AddListener(() => OnSortClicked?.Invoke());
            equipmentTypeDrop.onValueChanged.AddListener((value) => OnEquipmentTypeChanged?.Invoke(value));
            techniqueAndSkillTypeDrop.onValueChanged.AddListener((value) => OnTechniqueAndSkillTypeChanged?.Invoke(value));
            otherDrop.onValueChanged.AddListener((value) => OnOtherTypeChanged?.Invoke(value));

            searchItemField.onValueChanged.AddListener((string text) => OnSearchItemSubmit?.Invoke(text));
            searchItemField.onSubmit.AddListener((string text) => OnSearchItemSubmit?.Invoke(text));
        }
        public void ToggleMouseFollower(bool enable)
        {
            mouseFollower?.Toggle(enable);
        }

        public void SetFollowerData(Sprite sprite, int quantity)
        {
            mouseFollower?.SetData(sprite, quantity);
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
