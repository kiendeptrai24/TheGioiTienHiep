

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class InventorySystem : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private InventoryPagePresenter inventoryPage;
    [SerializeField] private List<InventoryItem> listItemDatas;
    protected override void Start()
    {
        inventoryPage?.SetInventoryData(listItemDatas);
    }
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