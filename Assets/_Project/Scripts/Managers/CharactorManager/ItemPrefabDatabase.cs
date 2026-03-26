using System;
using System.Collections.Generic;


public class ItemPrefabDatabase : Singleton<ItemPrefabDatabase>
{
    private InventoryCenterManager inventoryCenterManager;
    public event Action<List<ItemData>> OnPlayerPrefabChanged;
    protected override void Awake()
    {
        base.Awake();
        inventoryCenterManager = InventoryCenterManager.Instance;
        inventoryCenterManager.OnListItemDatasChampionChanged += OnListItemDatasChampionChanged;
    }
    
    public void OnListItemDatasChampionChanged(List<ItemData> list)
    {
        OnPlayerPrefabChanged?.Invoke(list);
    }
}