

using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private InventoryPagePresenter inventoryPage;
    [SerializeField] private List<InventoryItem> listItemDatas;
    public void LoadData(GameData _data)
    {
        foreach (var item in _data.itemDatas)
        {
            listItemDatas.Add(new InventoryItem(item));
        }
        inventoryPage?.SetInventoryData(listItemDatas);
    }

    public void SaveGame(ref GameData _data)
    {

    }
}