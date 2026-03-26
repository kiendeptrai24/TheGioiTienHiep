using Unity.Netcode;

public class PlayerClickable : EntityClickable
{
    public override void OnNetworkSpawn()
    {
        entityWorldType = EntityWorldType.Player;
    }
    public override void OnEntityClickedAccept(NetworkObject network)
    {
        BattleSimulatorRequest.Instance.RequestBattleSimulatorServerRpc(network.OwnerClientId, EntityNetId);
    }
}
