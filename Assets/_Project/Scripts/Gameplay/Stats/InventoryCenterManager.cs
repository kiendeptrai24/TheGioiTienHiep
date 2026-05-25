

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryCenterManager : Singleton<InventoryCenterManager>, ISaveable
{
    public ItemData playerCham;
    public HeroData championData;

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

    [SerializeField] private List<ItemData> listItemDatasUsed = new List<ItemData>();

    /// <summary>
    /// charactor use in team
    /// </summary>
    public List<ItemData> listItemDatasChampion = new List<ItemData>();
    public event Action OnLoadDataSuccessed;
    public int maxChampion = 4;
    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        ResetData();
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
    public event Action<ItemData> OnHeroItemChanged;
    public event Action<ItemData, string> OnItemUpdated;
    public event Action<List<ItemData>> OnItemUsedDataChanged;
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
    public List<ItemData> GetDatasUsed() => listItemDatasUsed;

    public void SetItemChampionData(List<ItemData> data)
    {
        listItemDatasChampion = data;
        OnListItemDatasChampionChanged?.Invoke(data);
    }
    public void ItemPlayerChanged(ItemData item)
    {
        playerCham = item;
        championData = playerCham as HeroData;
        OnItemPlayerChanged?.Invoke(item);
        OnItemUpdated?.Invoke(item, item.instanceId);
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
                var newItem = item.Clone();
                newItem.itemId = Guid.NewGuid().ToString();
                listItemDatas.Add(newItem);
                listItemDatasExisting.Add(newItem);
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
    public ItemData GetItemData(string itemId)
    {
        var item = listItemDatas.FirstOrDefault(i => i.itemId == itemId);
        if (item != null)
            return item;

        Debug.LogWarning($"Item with itemId {itemId} not found in inventory!");
        return null;
    }
    public void UpdateItemData(string itemId, ItemData updatedItem)
    {
        var existingItem = listItemDatas.FirstOrDefault(i => i.itemId == itemId);
        if (existingItem != null)
        {
            int index = listItemDatas.IndexOf(existingItem);
            listItemDatas[index] = updatedItem;

            if (listItemDatasExisting.Contains(existingItem))
            {
                int existingIndex = listItemDatasExisting.IndexOf(existingItem);
                listItemDatasExisting[existingIndex] = updatedItem;
            }

            ItemChange(updatedItem);
            ItemExistingChange(updatedItem);
            OnItemUpdated?.Invoke(updatedItem, existingItem.instanceId);

        }
        else
        {
            Debug.LogWarning($"Cannot update item. Item with itemId {updatedItem.itemId} not found in inventory!");
        }
    }
    public bool EquipData(ItemData item)
    {
        if (!listItemDatasExisting.Contains(item))
            return false;
        listItemDatasExisting.Remove(item);
        ItemExistingChange(item);
        return true;
    }
    public bool UnEquipData(ItemData item)
    {
        if (listItemDatasExisting.Contains(item))
            return false;
        listItemDatasExisting.Add(item);
        ItemExistingChange(item);
        return true;
    }
    public bool UseData(ItemData item)
    {
        if (!listItemDatas.Contains(item))
            return false;
        if (RemoveData(item) == false)
            return false;
        AddUsedData(item);
        ItemChange(item);
        return true;
    }
    public bool UnUseData(ItemData item)
    {
        if (!listItemDatasUsed.Contains(item))
            return false;
        if (AddData(item) == false)
            return false;
        RemoveUsedData(item);
        ItemChange(item);
        return true;
    }
    public bool AddUsedData(ItemData item)
    {
        if (listItemDatasUsed.Contains(item))
            return false;
        listItemDatasUsed.Add(item);
        OnItemUsedDataChanged?.Invoke(listItemDatasUsed);
        return true;
    }
    public bool RemoveUsedData(ItemData item)
    {
        if (listItemDatasUsed.Contains(item) == false)
            return false;
        listItemDatasUsed.Remove(item);
        OnItemUsedDataChanged?.Invoke(listItemDatasUsed);
        return true;
    }
    public void ItemExistingChange(ItemData item)
    {
        if (item is EquipmentData)
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
        if (item is EquipmentData)
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
            OnHeroItemChanged?.Invoke(item);
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
            if (item is EquipmentData)
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
        foreach (var item in _data.itemUsedDatas)
        {
            listItemDatasUsed.Add(item);
        }
        foreach (var item in _data.itemInTeamDatas)
        {
            if (item is HeroData heroData)
            {
                if (heroData.isCharactor)
                    ItemPlayerChanged(item);
            }
            listItemDatasChampion.Add(item);
        }
        OnListItemDatasChampionChanged?.Invoke(_data.itemInTeamDatas);
        // load item shop
        foreach (var item in _data.itemShopDatas)
        {
            listItemShopDatas.Add(item);
        }
        if (championData != null)
        {
            championData.healthPoint = _data.itemDataPoint.healthPoint;
            championData.manaPoint = _data.itemDataPoint.manaPoint;
            championData.spiritPoint = _data.itemDataPoint.spiritPoint;
            championData.moveSpeedPoint = _data.itemDataPoint.moveSpeedPoint;
            championData.spititRangePoint = _data.itemDataPoint.spititRangePoint;
            championData.physicalDamagePoint = _data.itemDataPoint.damagePoint;
            championData.physicalDefensePoint = _data.itemDataPoint.defensePoint;
        }
        OnLoadDataSuccessed?.Invoke();
    }
    public void SaveGame(ref GameData _data)
    {
        _data.itemDatas.Clear();
        _data.itemInTeamDatas.Clear();
        _data.itemDatas = listItemDatasExisting.ToList();
        _data.itemInTeamDatas = listItemDatasChampion.ToList();
        _data.itemUsedDatas = listItemDatasUsed;

        for (int i = 0; i < _data.itemCharacterDatas.Count; i++)
        {
            var itemcharacter = _data.itemCharacterDatas[i] as HeroData;

            if (itemcharacter is not HeroData) continue;

            var cham = playerCham as HeroData;

            if (cham == null) continue;

            if (itemcharacter.characterId == cham.characterId)
            {
                var player = playerCham as HeroData;
                _data.itemCharacterDatas[i] = player;
                break;
            }
        }
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
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