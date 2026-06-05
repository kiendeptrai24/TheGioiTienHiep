public class RealmUpgrade : IUpgradeable
{
    private HeroData heroData;
    private InventoryCenterManager inventoryCM;
    public RealmUpgrade(HeroData heroData, InventoryCenterManager inventoryCenterManager)
    {
        this.heroData = heroData;
        this.inventoryCM = inventoryCenterManager;
    }
    public void Upgrade()
    {
        var nextRealm = LevelUpDatabase.Instance.GetNextRealm(heroData.realmType);
        if (nextRealm != null)
        {
            heroData.realmId = nextRealm.realmId;
            heroData.realmType = nextRealm.realmType;
            heroData.realmData = nextRealm;
            inventoryCM.PlayerDataChanged(heroData);
        }
    }
}