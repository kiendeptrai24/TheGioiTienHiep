

using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCenterManager : Singleton<InventoryCenterManager>, ISaveable
{
    [SerializeField] private List<ItemData> listItemDatas = new List<ItemData>();
    public event Action<List<ItemData>> OnItemDataChanged;
    public event Action<List<ItemData>> OnItemEquitmentDataChanged;
    public event Action<List<ItemData>> OnItemSkillDataChanged;
    public event Action<List<ItemData>> OnItemTechniqueDataChanged;
    private bool isItemChange = false;
    private bool isEquitmentChange = false;
    private bool isSkillChange = false;
    private bool isTechniqueChange = false;
    //public event Action OnDataChanged;

    public void CheckDataChange()
    {
        if(isItemChange)
        {
            OnItemDataChanged?.Invoke(listItemDatas);
            isItemChange = false;
        }
        if(isEquitmentChange)
        {
            OnItemEquitmentDataChanged.Invoke(GetDataType(ItemType.Equipment));
            isEquitmentChange = false;
        }
        if(isSkillChange)
        {
            OnItemSkillDataChanged.Invoke(GetDataType(ItemType.Skill));
            isSkillChange = false;
        }
        if(isTechniqueChange)
        {
            OnItemTechniqueDataChanged.Invoke(GetDataType(ItemType.Technique));
        }
    }
    public List<ItemData> GetItemData()
    {
        return listItemDatas;
    }
    public bool AddData(ItemData item)
    {
        if (listItemDatas.Contains(item))
            return false;
        listItemDatas.Add(item);
        ItemChange(item);
        return true;
    }
    public bool RemoveData(ItemData item)
    {
        if (!listItemDatas.Contains(item))
            return false;
        listItemDatas.Remove(item);
        ItemChange(item);
        return true;
    }
    public void ItemChange(ItemData item)
    {
        if(item is EquitmentData)
            isEquitmentChange = true;
        else if(item is SkillData)
            isSkillChange = true;
        else if(item is TechniqueData)
            isTechniqueChange = true;
        
        isItemChange = true;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
    public List<ItemData> GetDataType(ItemType type)
    {
        List<ItemData> temp = new List<ItemData>();
        switch (type)
        {
            case ItemType.Material:
                return ListItemData(temp); 
            case ItemType.Equipment:
                return ListEquipmentData(temp);
            case ItemType.Technique:
                return ListTechniqueData(temp);
            case ItemType.Skill:
                return ListSkillData(temp);
        }
        return temp;
    }

    private List<ItemData> ListSkillData(List<ItemData> temp)
    {
        foreach (var item in listItemDatas)
        {
            if (item is SkillData)
                temp.Add(item);
        }
        return temp;
    }

    private List<ItemData> ListTechniqueData(List<ItemData> temp)
    {
        foreach (var item in listItemDatas)
        {
            if (item is TechniqueData)
                temp.Add(item);
        }
        return temp;
    }

    private List<ItemData> ListItemData(List<ItemData> temp)
    {
        foreach (var item in listItemDatas)
        {
            if (item is ItemData)
                temp.Add(item);
        }
        return temp;
    }

    private List<ItemData> ListEquipmentData(List<ItemData> temp)
    {
        foreach (var item in listItemDatas)
        {
            if (item is EquitmentData)
                temp.Add(item);
        }
        return temp;
    }

    public void LoadData(GameData _data)
    {
        foreach (var item in _data.itemDatas)
        {
            listItemDatas.Add(item);
        }
    }
    public void SaveGame(ref GameData _data)
    {
        
    }
}