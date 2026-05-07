using System;
using UnityEngine;
using WorldMap.Travel;

public class ItemMapWorld : TGTHNetworkBehaviour
{
    private Canvas canvas;
    [SerializeField] private string instanceId;
    private ItemResourseData itemData;
    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponentInChildren<Canvas>();
        LoadComponent();
        HideIcon();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ResetItemData();
    }
    public ItemResourseData GetItemData()
    {
        return itemData;
    }
    public void ResetItemData()
    {
        itemData = GameDataCenterManager.Instance.GetItemById(instanceId) as ItemResourseData;
        if (itemData != null)
        {
            itemData.position = transform.position;
        }
    }
    public void ShowIcon() => canvas.enabled = true;
    public void HideIcon() => canvas.enabled = false;
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}
