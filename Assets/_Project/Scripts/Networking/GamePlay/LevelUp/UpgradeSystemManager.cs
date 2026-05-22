using System;
using System.Collections.Generic;

public class UpgradeSystemManager : Singleton<UpgradeSystemManager>
{
    public static string RealmUpgradeId = "Realm";
    private readonly Dictionary<string, IUpgradeable> upgrades = new();
    protected override void Awake()
    {
        base.Awake();
        InventoryCenterManager.Instance.OnLoadDataSuccessed += OnLoadDataSuccessed;
    }

    private void OnLoadDataSuccessed()
    {
        var inventoryCM = InventoryCenterManager.Instance;
        if (inventoryCM == null)
            return;
        var heroData = InventoryCenterManager.Instance.playerCham as HeroData;
        if (heroData == null)
            return;
        upgrades.Add(RealmUpgradeId, new RealmUpgrade(heroData, inventoryCM));
    }

    public bool TryUpgrade(string upgradeId)
    {
        if (!upgrades.TryGetValue(upgradeId, out IUpgradeable upgrade))
            return false;

        upgrade.Upgrade();
        return true;
    }
}