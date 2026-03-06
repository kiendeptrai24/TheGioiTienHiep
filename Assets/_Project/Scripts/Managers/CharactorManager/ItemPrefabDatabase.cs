using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class ItemPrefabDatabase : Singleton<ItemPrefabDatabase>
{
    [SerializeField] private List<StatsData> itemPrefabs = new();
    private Dictionary<string, GameObject> lookup;
    private InventoryCenterManager inventoryCenterManager;
    private PlayerNetManager playerNetManager;
    public event Action<List<GameObject>> OnPlayerPrefabChanged;
    protected override void Awake()
    {
        base.Awake();
        lookup = new Dictionary<string, GameObject>();
        inventoryCenterManager = InventoryCenterManager.Instance;
        playerNetManager = PlayerNetManager.Instance;
        inventoryCenterManager.OnListItemDatasChampionChanged += OnListItemDatasChampionChanged;

        playerNetManager.OnPlayerExiststed += LoadPrefab;
        foreach (var entry in itemPrefabs)
        {
            if (!lookup.ContainsKey(entry.heroData.itemId))
            {
                lookup.Add(entry.heroData.itemId, entry.gameObject);
            }
        }
    }

    private void OnListItemDatasChampionChanged(List<ItemData> list)
    {
        OnPlayerPrefabChanged?.Invoke(GetPrefab());
    }

    private void LoadPrefab(NetworkObject playerNet)
    {
       OnPlayerPrefabChanged?.Invoke(GetPrefab());
    }

    public List<GameObject> GetPrefab()
    {
        var prefabs = new List<GameObject>();
        foreach (var item in inventoryCenterManager.listItemDatasChampion)
        {
            if (lookup.TryGetValue(item.itemId, out var prefab))
            {
                prefab.GetComponent<StatsData>().heroData = item;
                prefabs.Add(prefab);
            }
        }
        return prefabs;
    }
}