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
        EntityAcceptServerRpc(network.NetworkObjectId, NetworkObjectId);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void EntityAcceptServerRpc(ulong heroId, ulong enemyId)
    {
        if (!IsServer) return;
        BattleSimulatorRequest.Instance.RequestBattleSimulator(heroId, enemyId, (win) =>
        {
            if (win)
            {
                NotifyResultClientRpc(
                $"You won ",
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { heroId }
                    }
                });
            }
        });
    }
    [ClientRpc]
    private void NotifyResultClientRpc(string message, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log(message);
    }

}
