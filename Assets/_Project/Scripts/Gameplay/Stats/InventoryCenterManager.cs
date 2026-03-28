

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryCenterManager : Singleton<InventoryCenterManager>, ISaveable
{
    public bool getDataPreset;
    [SerializeField] private HeroPreset heroPreset;
    public ItemData playerCham;

    /// <summary>
    /// all item
    /// </summary>
    public List<ItemData> allItemDatas = new List<ItemData>();

    /// <summary>
    /// Item Shop
    /// </summary>
    [SerializeField] private List<ItemData> listItemShopDatas = new List<ItemData>();

    /// <summary>
    /// item player own
    /// </summary>
    [SerializeField] private List<ItemData> listItemDatas = new List<ItemData>();

    /// <summary>
    /// item exist when player use
    /// </summary>
    [SerializeField] private List<ItemData> listItemDatasExisting = new List<ItemData>();

    /// <summary>
    /// charactor use in team
    /// </summary>
    public List<ItemData> listItemDatasChampion = new List<ItemData>();

    public int maxChampion = 4;
    override protected void Awake()
    {
        base.Awake();
        if (getDataPreset)
        {
            playerCham = heroPreset.GetItemData();
        }
        LoadComponent();
    }
    #region Event General

    public event Action<List<ItemData>> OnItemDataChanged;
    public event Action<List<ItemData>> OnListItemDatasChampionChanged;
    public event Action<List<ItemData>> OnItemEquitmentDataChanged;
    public event Action<List<ItemData>> OnItemSkillDataChanged;
    public event Action<List<ItemData>> OnItemChampionDataChanged;
    public event Action<List<ItemData>> OnItemTechniqueDataChanged;

    public event Action<List<ItemData>> OnItemExistingDataChanged;
    public event Action<List<ItemData>> OnItemExistingEquitmentDataChanged;
    public event Action<List<ItemData>> OnItemExistingSkillDataChanged;
    public event Action<List<ItemData>> OnItemExistingChampionDataChanged;
    public event Action<List<ItemData>> OnItemExistingTechniqueDataChanged;
    public event Action<ItemData> OnItemChanged;
    #endregion

    public event Action<ItemData> OnItemPlayerChanged;
    public event Action<List<ItemData>> OnItemCharacterChanged;

    private bool isItemChange = false;
    private bool isEquitmentChange = false;
    private bool isSkillChange = false;
    private bool isChampionChange = false;
    private bool isTechniqueChange = false;
    //public event Action OnDataChanged;
    public List<ItemData> GetItemShopData() => listItemShopDatas;
    public List<ItemData> GetDatasChampion() => listItemDatasChampion;

    public void SetItemChampionData(List<ItemData> data)
    {
        listItemDatasChampion = data;
        OnListItemDatasChampionChanged?.Invoke(data);
    }
    public void ItemPlayerChanged(ItemData item)
    {
        playerCham = item;
        OnItemPlayerChanged?.Invoke(item);
    }
    public void CheckDataChange()
    {
        if (isItemChange)
        {
            OnItemDataChanged?.Invoke(listItemDatas);
            isItemChange = false;
        }
        if (isEquitmentChange)
        {
            OnItemEquitmentDataChanged?.Invoke(GetDataType(ItemType.Equipment));
            isEquitmentChange = false;
        }
        if (isChampionChange)
        {
            OnItemChampionDataChanged?.Invoke(GetDataType(ItemType.Champion));
            isChampionChange = false;
        }
        if (isSkillChange)
        {
            OnItemSkillDataChanged?.Invoke(GetDataType(ItemType.Skill));
            isSkillChange = false;
        }
        if (isTechniqueChange)
        {
            OnItemTechniqueDataChanged?.Invoke(GetDataType(ItemType.Technique));
            isTechniqueChange = false;
        }
    }
    public void CheckExistingDataChange()
    {
        if (isItemChange)
        {
            OnItemExistingDataChanged?.Invoke(listItemDatasExisting);
            isItemChange = false;
        }
        if (isEquitmentChange)
        {
            OnItemExistingEquitmentDataChanged?.Invoke(GetDataType(ItemType.Equipment, true));
            isEquitmentChange = false;
        }
        if (isChampionChange)
        {
            OnItemExistingChampionDataChanged?.Invoke(GetDataType(ItemType.Champion, true));
            isChampionChange = false;
        }
        if (isSkillChange)
        {
            OnItemExistingSkillDataChanged?.Invoke(GetDataType(ItemType.Skill, true));
            isSkillChange = false;
        }
        if (isTechniqueChange)
        {
            OnItemExistingTechniqueDataChanged?.Invoke(GetDataType(ItemType.Technique, true));
            isTechniqueChange = false;
        }
    }
    public List<ItemData> GetItemData()
    {
        return listItemDatas;
    }
    public bool AddData(ItemData item, int quantity = 1)
    {
        listItemDatas.Add(item);
        listItemDatasExisting.Add(item);
        if (quantity > 1)
        {
            for (int i = 0; i < quantity - 1; i++)
            {
                listItemDatas.Add(item.Clone());
                listItemDatasExisting.Add(item.Clone());
            }
        }

        // Notify đúng object
        ItemChange(item);
        ItemExistingChange(item);

        return true;
    }
    public bool RemoveData(ItemData item)
    {
        if (!listItemDatas.Contains(item))
            return false;

        listItemDatas.Remove(item);
        listItemDatasExisting.Remove(item);

        ItemChange(item);
        ItemExistingChange(item);
        return true;
    }
    public bool UseData(ItemData item)
    {
        if (!listItemDatasExisting.Contains(item))
            return false;
        listItemDatasExisting.Remove(item);
        ItemExistingChange(item);
        return true;
    }
    public bool UnUseData(ItemData item)
    {
        if (listItemDatasExisting.Contains(item))
            return false;
        listItemDatasExisting.Add(item);
        ItemExistingChange(item);
        return true;
    }
    public void ItemExistingChange(ItemData item)
    {
        if (item is EquitmentData)
        {
            isEquitmentChange = true;
        }
        else if (item is SkillData)
        {
            isSkillChange = true;
        }
        else if (item is TechniqueData)
        {
            isTechniqueChange = true;
        }
        else if (item is HeroData)
        {
            isChampionChange = true;
        }

        isItemChange = true;
        CheckExistingDataChange();
    }
    public void ItemChange(ItemData item)
    {
        if (item is EquitmentData)
        {
            isEquitmentChange = true;
        }
        else if (item is SkillData)
        {
            isSkillChange = true;
        }
        else if (item is TechniqueData)
        {
            isTechniqueChange = true;
        }
        else if (item is HeroData)
        {
            isChampionChange = true;
        }
        else if (item is HeroData)
        {
            isItemChange = true;
            OnItemChanged?.Invoke(item);
        }

        isItemChange = true;
        CheckDataChange();
    }

    public List<ItemData> GetDataType(ItemType type, bool onlyExisting = false)
    {
        var listDatas = onlyExisting ? listItemDatasExisting : listItemDatas;
        switch (type)
        {
            case ItemType.Material:
                return ListItemData(listDatas);
            case ItemType.Equipment:
                return ListEquipmentData(listDatas);
            case ItemType.Technique:
                return ListTechniqueData(listDatas);
            case ItemType.Skill:
                return ListSkillData(listDatas);
            case ItemType.Champion:
                return ListChampionData(listDatas);
        }
        return listDatas;
    }

    public List<ItemData> GetAllDataType(ItemType type)
    {
        List<ItemData> listDatas = allItemDatas;

        switch (type)
        {
            case ItemType.Material:
                return ListItemData(listDatas);
            case ItemType.Equipment:
                return ListEquipmentData(listDatas);
            case ItemType.Technique:
                return ListTechniqueData(listDatas);
            case ItemType.Skill:
                return ListSkillData(listDatas);
            case ItemType.Champion:
                return ListChampionData(listDatas);
        }
        return listDatas;
    }

    private List<ItemData> ListChampionData(List<ItemData> temps)
    {
        List<ItemData> temp = new();
        foreach (var item in temps)
        {
            if (item is HeroData)
                temp.Add(item);
        }
        return temp;
    }

    private List<ItemData> ListSkillData(List<ItemData> temps)
    {
        List<ItemData> temp = new();
        foreach (var item in temps)
        {
            if (item is SkillData)
                temp.Add(item);
        }
        return temp;
    }

    private List<ItemData> ListTechniqueData(List<ItemData> temps)
    {
        List<ItemData> temp = new();
        foreach (var item in temps)
        {
            if (item is TechniqueData)
                temp.Add(item);
        }
        return temp;
    }

    private List<ItemData> ListItemData(List<ItemData> temps)
    {
        List<ItemData> temp = new();
        foreach (var item in temps)
        {
            if (item is ItemData)
                temp.Add(item);
        }
        return temp;
    }

    private List<ItemData> ListEquipmentData(List<ItemData> temps)
    {
        List<ItemData> temp = new();
        foreach (var item in temps)
        {
            if (item is EquitmentData)
                temp.Add(item);
        }
        return temp;
    }

    public void LoadData(GameData _data)
    {
        // load item you is owned
        foreach (var item in _data.itemDatas)
        {
            if (item is HeroData heroData)
            {
                if (heroData.isCharactor)
                    ItemPlayerChanged(item);
            }
            listItemDatas.Add(item);
            listItemDatasExisting.Add(item);
        }
        // load all item and all character item
        foreach (var item in _data.allItemsDatas)
        {
            allItemDatas.Add(item);
        }
        foreach (var item in _data.itemDatasInTeam)
        {
            if (item is HeroData heroData)
            {
                if (heroData.isCharactor)
                    ItemPlayerChanged(item);
            }
            listItemDatasChampion.Add(item);
        }
        // load item shop
        foreach (var item in _data.itemShopDatas)
        {
            listItemShopDatas.Add(item);
        }
    }
    public void SaveGame(ref GameData _data)
    {
        _data.itemDatas.Clear();
        _data.itemDatasInTeam.Clear();

        _data.itemDatas = listItemDatasExisting;
        _data.itemDatasInTeam = listItemDatasChampion;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        ResetData();
    }

    private void ResetData()
    {
        listItemDatas.Clear();
        listItemDatasExisting.Clear();
        listItemShopDatas.Clear();
        listItemDatasChampion.Clear();
        allItemDatas.Clear();
    }
}