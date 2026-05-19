using System.Collections.Generic;
using UnityEngine;

public class PlayerSpiritRange : TGTHNetworkBehaviour
{
    [SerializeField] private float intervelTime = 1f;

    private StatsData statsData;
    private float lastTime;

    private readonly HashSet<ResourceNode> currentItemsInRange = new();

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        statsData = GetComponent<StatsData>();
    }

    void Update()
    {
        if (Time.time - lastTime < intervelTime)
            return;

        lastTime = Time.time;
        CheckSpiritRange();
    }

    private void CheckSpiritRange()
    {
        if (statsData == null) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, statsData.SpiritRange);

        HashSet<ResourceNode> newItemsInRange = new();

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<ResourceNode>(out var itemMapWorld))
            {
                newItemsInRange.Add(itemMapWorld);

                if (!currentItemsInRange.Contains(itemMapWorld))
                {
                    itemMapWorld.ShowIcon();
                }
            }
        }

        foreach (var oldItem in currentItemsInRange)
        {
            if (!newItemsInRange.Contains(oldItem))
            {
                oldItem.HideIcon();
            }
        }

        currentItemsInRange.Clear();
        foreach (var item in newItemsInRange)
        {
            currentItemsInRange.Add(item);
        }
    }
}