
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMap.Travel;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] private LayerMask ignoreLayerMask;
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

        Collider[] colliders = Physics.OverlapSphere(position, range, ~ignoreLayerMask);

        foreach (var col in colliders)
        {
            var itemWorld = col.gameObject.transform.root.GetComponent<IDataMapWorld>();
            if (itemWorld != null)
            {
                result.Add(itemWorld.GetData());
            }
        }
        return result;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}