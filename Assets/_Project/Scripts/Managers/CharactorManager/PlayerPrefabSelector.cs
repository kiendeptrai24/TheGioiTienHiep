

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[Serializable]
public class PlayerPrefabEntry
{
    public string id;
    public GameObject prefab;
}
public class PlayerPrefabSelector : Singleton<PlayerPrefabSelector>
{
    public List<PlayerPrefabEntry> prefabList = new List<PlayerPrefabEntry>();
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();
    private ItemData itemData;
    protected override void Awake()
    {
        base.Awake();
        foreach (var item in prefabList)
        {
            prefabMap.Add(item.id, item.prefab);
        }
    }
    public void SetItemData(ItemData _itemData)
    {
        itemData = _itemData;
    }
    public ItemData GetItemData()
    {
        return itemData;
    }
    public GameObject GetSelectedPrefab(string id)
    {
        GameObject playerPrefab = null;
        if (prefabMap.ContainsKey(id))
        {
            playerPrefab = prefabMap[id];
        }
        return playerPrefab;
    }
}