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
                Debug.Log("heroId win");
                SpawnMonter.Instance.RemoveNetObject(NetworkObject);
            }
            else
            {
                var playerNet = NetworkManager.SpawnManager.SpawnedObjects[heroId];
                if (playerNet)
                {
                    Debug.Log("playerNet");
                    playerNet.transform.position = new Vector3(500, 0, 440);
                    playerNet.transform.rotation = Quaternion.identity;
                }
                Debug.Log("enemyId win");

                NotifyResultClientRpc(
                $"You lost ",
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { enemyId }
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
