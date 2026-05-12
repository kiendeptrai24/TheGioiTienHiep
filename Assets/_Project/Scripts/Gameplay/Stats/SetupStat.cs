

using System;
using UnityEngine;

public class SetupStat : TGTHMonoBehaviour
{
    private StatManager statManager;
    private InventoryCenterManager inventoryCM;
    protected override void Awake()
    {
        base.Awake();
        statManager = StatManager.Instance;
        inventoryCM = InventoryCenterManager.Instance;
        inventoryCM.OnItemPlayerChanged += OnItemPlayerChanged;
    }

    private void OnItemPlayerChanged(ItemData data)
    {
        statManager.SetStat(data);
    }
}