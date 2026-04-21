
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMap.Travel;

public class ResourceManager : Singleton<ResourceManager>
{
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
    }
    public List<ItemData> GetItemsRange(Vector3 position, float range)
    {
        var result = new List<ItemData>();

        Collider[] colliders = Physics.OverlapSphere(position, range);

        foreach (var col in colliders)
        {
            var itemWorld = col.GetComponent<ItemMapWorld>();
            if (itemWorld != null)
            {
                Debug.Log(itemWorld.gameObject.name);
                result.Add(itemWorld.GetItemData());
            }
        }
        return result;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}