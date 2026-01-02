
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class EquipmentBasePageView : TGTHMonoBehaviour 
{
    [Header("UI References")]
    public Button sortBtn;
    public Button showAllItemsBtn;
    public TMP_Dropdown eqipmenttypeDrop;
    public TMP_Dropdown qualityTypeDrop;

    public RectTransform contentPanel;
    public UIInventoryItem itemPrefab;
    public MouseFollower mouseFollower;
    public List<UIEquipmentSlot> listOfEquitmentItems = new List<UIEquipmentSlot>();   
    public Dictionary<EquipmentType, UIItemSlotBase> equipmentSlotsDictionary; 
    public List<UIItemSlotBase> listOfUIItemsInInventory = new List<UIItemSlotBase>();
    public List<UIItemSlotBase> listOfUIItems = new List<UIItemSlotBase>();
    protected int currentlyDraggedItemIndex = -1;

    public event Action OnRefreshClicked;
    public event Action OnSortClicked;

    protected override void Awake()
    {
        sortBtn.onClick.AddListener(() => OnSortClicked?.Invoke());
        showAllItemsBtn.onClick.AddListener(() => OnRefreshClicked?.Invoke());
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
            if(i >= 50) return;
            listOfUIItems[i].SetItem(listItemDatas[i]);
        }
    }
    public virtual void ShowEquipmentItems(ItemData data)
    {
        HeroData heroData = data as HeroData;
        if (heroData == null) return;
        foreach (var item in listOfEquitmentItems)
        {
            item.ResetData();
        }
        for (int i = 0; i < heroData.equitmentDatas.Count; i++)
        {
            equipmentSlotsDictionary[heroData.equitmentDatas[i].equipmentType].SetItem(new InventoryItem(heroData.equitmentDatas[i]));
        }
    }
    public void ShowAllItemInInventory(List<InventoryItem> listItemDatas)
    {
        if (listItemDatas == null) return;
        if (listOfUIItems.Count < listItemDatas.Count) return;
        for (int i = 0; i < listOfUIItemsInInventory.Count; i++)
        {
            if(i < listItemDatas.Count)
            {
                listOfUIItemsInInventory[i].SetItem(listItemDatas[i]);
            }
            else
            {
                listOfUIItemsInInventory[i].ResetData();
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