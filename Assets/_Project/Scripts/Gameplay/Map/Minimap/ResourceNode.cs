using System;
using UnityEngine;

public class ResourceNode : TGTHNetworkBehaviour, IDataMapWorld
{
    private Canvas canvas;
    [SerializeField] private string instanceId;
    [SerializeField] private ItemResourseData itemData;
    private bool isDataReady = false;

    public event Action<ItemData> OnDataReady;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponentInChildren<Canvas>();
        HideIcon();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        InitializeItemData();
    }
    private void InitializeItemData()
    {
        itemData = GameDataCenterManager.Instance.GetItemById(instanceId).Clone() as ItemResourseData;
        itemData.position = transform.position;
        itemData.resourceId = Guid.NewGuid().ToString();
        if (itemData != null)
        {
            itemData.position = transform.position;
            OnDataReady?.Invoke(itemData);
            isDataReady = true;
        }
    }
    public void ShowIcon() => canvas.enabled = true;
    public void HideIcon() => canvas.enabled = false;
    public ItemData GetData() => itemData;
    public bool IsDataReady() => isDataReady;
}