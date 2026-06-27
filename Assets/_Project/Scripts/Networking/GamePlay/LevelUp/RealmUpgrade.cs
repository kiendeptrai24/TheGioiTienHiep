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
        Upgrade(heroData?.characterId);
    }

    public void Upgrade(string characterId)
    {
        HeroData targetHero = null;

        if (string.IsNullOrEmpty(characterId) == false)
            targetHero = inventoryCM.GetHeroByCharacterId(characterId);

        if (targetHero == null)
            targetHero = heroData;

        if (targetHero == null)
            targetHero = inventoryCM.playerCham as HeroData;

        if (targetHero == null)
            return;

        var nextRealm = LevelUpDatabase.Instance.GetNextRealm(targetHero.realmType);
        if (nextRealm != null)
        {
            targetHero.realmId = nextRealm.realmId;
            targetHero.realmType = nextRealm.realmType;
            targetHero.realmData = nextRealm;

            inventoryCM.UpdateItemData(targetHero.itemId, targetHero);

            if (inventoryCM.playerCham is HeroData currentHero && currentHero.characterId == targetHero.characterId)
            {
                inventoryCM.PlayerDataChanged(targetHero);
                inventoryCM.NotifyListItemDatasChampionChanged();
            }
        }
    }
}