

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class EquipmentLoadGame : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private EquipmentBasePagePresenter equipmentPage;
    [SerializeField] private List<InventoryItem> listItemDatas;
    protected override void Awake()
    {
        equipmentPage?.SetInventoryData(listItemDatas);
    }
    public void LoadData(GameData _data)
    {
        foreach (var item in _data.itemDatas)
        {
            if (item is EquitmentData)
                listItemDatas.Add(new InventoryItem(item));
        }
        equipmentPage?.SetInventoryData(listItemDatas);
    }

    public void SaveGame(ref GameData _data)
    {

    }
}