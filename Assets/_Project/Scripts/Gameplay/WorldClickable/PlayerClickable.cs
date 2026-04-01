using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerClickable : EntityClickable
{
    public override void OnNetworkSpawn()
    {
        entityWorldType = EntityWorldType.Player;
    }
    public override void OnEntityClickedAccept(NetworkObject network)
    {
        EntityAcceptServerRpc(network.NetworkObjectId, EntityNetId);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void EntityAcceptServerRpc(ulong heroId, ulong enemyId)
    {
        if (!IsServer) return;
        BattleSimulatorRequest.Instance.RequestBattleSimulator(heroId, enemyId, (win) =>
        {
            if (win)
            {
                Debug.Log("You won");
                RewardsAndPunishments(heroId, enemyId);
            }
            else
            {
                Debug.Log("You lost");
            }
        });
    }
    private void RewardsAndPunishments(ulong heroId, ulong enemyId)
    {
        if (!IsServer) return;

        var heroObject = NetworkManager.SpawnManager.SpawnedObjects[heroId];
        var enemyObject = NetworkManager.SpawnManager.SpawnedObjects[enemyId];

        if (heroObject == null || enemyObject == null) return;

        // phải là player hết
        if (!heroObject.IsPlayerObject || !enemyObject.IsPlayerObject) return;

        var heroResource = heroObject.GetComponent<ResourceStorage>();
        var enemyResource = enemyObject.GetComponent<ResourceStorage>();

        if (heroResource == null || enemyResource == null) return;

        ulong enemyCoins = enemyResource.Coins.Value;

        if (enemyCoins < 100) return;

        ulong reward = (ulong)(enemyCoins * 0.7f);

        heroResource.PlusCost(reward);
        enemyResource.MinusCost(reward);

        var heroClientId = heroObject.OwnerClientId;
        var enemyClientId = enemyObject.OwnerClientId;

        NotifyResultClientRpc(
            $"Bạn đã thắng! nhận dược {reward} Linh Thạch! 70% tổng số Linh Thạch nhận từ đối thủ",
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { heroClientId }
                }
            });

        NotifyResultClientRpc(
            $"Bạn đã thua! bị trừ {reward} Linh Thạch! 70% tổng số Linh Thạch bị trừ",
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { enemyClientId }
                }
            });
    }
    [ClientRpc]
    private void NotifyResultClientRpc(string message, ClientRpcParams clientRpcParams = default)
    {
        TopNotificationUI.Instance.Show(message);
    }
}
