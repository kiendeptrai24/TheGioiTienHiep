using Unity.Netcode;
using UnityEngine;

public class MonsterClickable : EntityClickable
{
    public override void OnEntityClickedAccept(NetworkObject network)
    {
        BattleSimulatorRequest.Instance.RequestBattleSimulatorServerRpc(network.OwnerClientId, EntityNetId);
    }
}
