

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class InventoryLoadGame : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private InventoryPagePresenter presenter;
    [SerializeField] private InventoryUseSystem inventoryUseSystem;
    [SerializeField] private List<InventoryItem> listItemDatas;
    protected override void Start()
    {
        presenter?.SetInventoryData(listItemDatas);
    }
    public void LoadData(GameData _data)
    {
        foreach (var item in _data.itemDatas)
        {
            listItemDatas.Add(new InventoryItem(item));
        }
        presenter?.SetInventoryData(listItemDatas);
        inventoryUseSystem.SetInventoryData(listItemDatas);
    }
    public void SaveGame(ref GameData _data)
    {

    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        presenter = GetComponent<InventoryPagePresenter>();
        inventoryUseSystem = GetComponent<InventoryUseSystem>();
    }
}