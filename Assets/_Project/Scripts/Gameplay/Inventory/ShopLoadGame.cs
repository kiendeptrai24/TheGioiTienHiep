using System.Collections.Generic;
using UnityEngine;

public class ShopLoadGame : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private ShopPageManager shopPageManager;
    [SerializeField] private List<InventoryItem> listItemDatas;
    public void LoadData(GameData _data)
    {
        foreach (var item in _data.itemDatas)
        {
            listItemDatas.Add(new InventoryItem(item));
        }
        shopPageManager?.SetInventoryData(listItemDatas);
    }

    public void SaveGame(ref GameData _data)
    {

    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        shopPageManager = GetComponent<ShopPageManager>();
    }
}