

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class TechniqueLoadGame : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private TechniquePresenter presenter;
    [SerializeField] private List<InventoryItem> listItemDatas;
    protected override void Awake()
    {
        presenter?.SetInventoryData(listItemDatas);
    }
    public void LoadData(GameData _data)
    {
        foreach (var item in _data.itemDatas)
        {
            if (item is TechniqueData)
                listItemDatas.Add(new InventoryItem(item));
        }
        presenter?.SetInventoryData(listItemDatas);
    }

    public void SaveGame(ref GameData _data)
    {

    }
}