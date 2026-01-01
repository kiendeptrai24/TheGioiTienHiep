

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class HeroLoadGame : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private HeroPresenter presenter;
    [SerializeField] private List<InventoryItem> listItemDatas;
    protected override void Awake()
    {
        presenter?.SetInventoryData(listItemDatas);
    }
    public void LoadData(GameData _data)
    {
        foreach (var item in _data.itemDatas)
        {
            if (item is HeroData)
                listItemDatas.Add(new InventoryItem(item));
        }
        presenter?.SetInventoryData(listItemDatas);
    }

    public void SaveGame(ref GameData _data)
    {

    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        presenter = GetComponent<HeroPresenter>();
    }
}