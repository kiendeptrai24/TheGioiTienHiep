


using UnityEngine;

public class ChampionSetup : TGTHNetworkBehaviour
{
    private ItemData championData;
    private StatsData statsData;
    private InventoryCenterManager inventoryCM;
    private StatManager statManager;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        statsData = GetComponent<StatsData>();
        championData = inventoryCM.championData;
        statsData.SetUpItem(championData);
        statManager.SetStat(championData);
    }
}