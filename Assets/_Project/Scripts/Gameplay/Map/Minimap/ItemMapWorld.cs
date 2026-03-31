using System;
using UnityEngine;
using WorldMap.Travel;

public class ItemMapWorld : TGTHNetworkBehaviour
{
    [SerializeField] private ItemResourcePreset itemDataPreset;
    private ItemData itemData;
    protected override void Awake()
    {
        base.Awake();

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ResetItemData();
    }
    public ItemData GetItemData()
    {
        return itemData;
    }
    public void ResetItemData()
    {
        itemData = itemDataPreset.GetItemData();
        var itemResources = itemData as ItemResourseData;
        itemResources.position = transform.position;
        Debug.Log(itemResources.position);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}
