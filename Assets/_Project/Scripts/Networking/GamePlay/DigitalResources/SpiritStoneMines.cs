using UnityEngine;

public class SpiritStoneMines : TGTHNetworkBehaviour
{
    private MineNetworkState networkState;
    private MineOwnershipSystem ownership;
    private MineProductionSystem production;
    private MineOfflineRewardSystem offline;
    private MineRosterLinker rosterLinker;

    [SerializeField]
    private SpiritStoneMineData miningData;

    private ResourceStorage ownerStorage;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkState = new MineNetworkState();

        ownership =
            new MineOwnershipSystem(
                networkState);

        production =
            new MineProductionSystem(
                miningData,
                networkState);

        offline =
            new MineOfflineRewardSystem(
                miningData);

        rosterLinker =
            new MineRosterLinker(
                GetComponent<PlayerBattleRoster>());
    }
    private void Update()
    {
        if (!IsServer)
            return;

        if (!ownership.HasOwner())
            return;

        if (ownership.IsOnline())
        {
            production.Tick(
                NetworkManager.ServerTime.Time,
                ownerStorage
            );
        }
    }
}