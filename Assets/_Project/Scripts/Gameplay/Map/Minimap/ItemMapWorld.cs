using System;
using UnityEngine;
using WorldMap.Travel;

public class ItemMapWorld : TGTHMonoBehaviour
{
    [SerializeField] private ItemResourcePreset itemDataPreset;
    public Destination destination;
    public event Action<Destination> OnItemInteract;
    protected override void Awake()
    {
        base.Awake();
        destination.spawnPoint = transform;
    }
    protected override void Start()
    {
        base.Start();
    }
    public void ItemInteract() => OnItemInteract?.Invoke(destination);

    public ItemData GetItemData()
    {
        destination.itemData = itemDataPreset.GetItemData();
        var itemResources = destination.itemData as ItemResourseData;
        itemResources.position = transform.position;
        return destination.itemData;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}
