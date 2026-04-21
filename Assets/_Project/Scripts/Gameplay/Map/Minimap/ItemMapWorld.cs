using System;
using UnityEngine;
using WorldMap.Travel;

public class ItemMapWorld : TGTHNetworkBehaviour
{
    [SerializeField] private ItemResourcePreset itemDataPreset;
    private Canvas canvas;
    private ItemData itemData;
    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponentInChildren<Canvas>();
        LoadComponent();
        HideIcon();
        ResetItemData();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
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
    }
    public void ShowIcon() => canvas.enabled = true;
    public void HideIcon() => canvas.enabled = false;
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}
