using System;
using System.Collections.Generic;

public class UpgradeSystemManager : Singleton<UpgradeSystemManager>
{
    public static string RealmUpgradeId = "Realm";
    public event Action<bool> OnRealmUpgrade;
    private readonly Dictionary<string, IUpgradeable> upgrades = new();
    protected override void Awake()
    {
        base.Awake();
        InventoryCenterManager.Instance.OnLoadDataSuccessed += OnLoadDataSuccessed;
    }
    protected void OnDestroy()
    {
        if (InventoryCenterManager.Instance != null)
            InventoryCenterManager.Instance.OnLoadDataSuccessed -= OnLoadDataSuccessed;

    }
    private void OnLoadDataSuccessed()
    {
        var inventoryCM = InventoryCenterManager.Instance;
        if (inventoryCM == null)
            return;
        var heroData = InventoryCenterManager.Instance.playerCham as HeroData;
        if (heroData == null)
            return;
        if (upgrades.ContainsKey(RealmUpgradeId))
        {
            upgrades.Remove(RealmUpgradeId);
        }
        upgrades.Add(RealmUpgradeId, new RealmUpgrade(heroData, inventoryCM));
    }

    public bool TryUpgrade(string upgradeId)
    {
        if (!upgrades.TryGetValue(upgradeId, out IUpgradeable upgrade))
            return false;

        upgrade.Upgrade();
        OnRealmUpgrade?.Invoke(true);
        return true;
    }
}