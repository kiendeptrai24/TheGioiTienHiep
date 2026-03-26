using Unity.Netcode;
using UnityEngine;

public class MonsterClickable : EntityClickable
{
    public override void OnNetworkSpawn()
    {
        entityWorldType = EntityWorldType.Monster;
    }
    public override void OnEntityClickedAccept(NetworkObject network)
    {
        Debug.Log("Accept");
        BattleSimulatorRequest.Instance.RequestBattleSimulatorServerRpc(network.NetworkObjectId, EntityNetId);
    }
}
