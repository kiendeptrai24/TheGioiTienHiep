using System;
using UnityEngine;
using WorldMap.Travel;

public class ItemMapWorld : TGTHMonoBehaviour
{
    [SerializeField] private Destination destination;
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

    protected override void LoadComponent()
    {
        base.LoadComponent();
        destination.spawnPoint = transform;

    }
}
