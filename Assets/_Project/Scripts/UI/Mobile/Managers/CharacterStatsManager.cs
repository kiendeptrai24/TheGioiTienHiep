

using System;
using UnityEngine;

public class CharacterStatsManager : Singleton<CharacterStatsManager>
{
    public StatsData stats;
    private InventoryCenterManager inventoryCenterManager;
    private ItemData playerCham;
    protected override void Awake()
    {
        base.Awake();
        inventoryCenterManager = InventoryCenterManager.Instance;
        playerCham = inventoryCenterManager.playerCham;
        inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;
        OnItemPlayerChanged(playerCham);
    }

    private void OnItemPlayerChanged(ItemData data)
    {
        stats.SetUpItem(playerCham);
    }

    protected override void Start()
    {
        base.Start();
    }
}