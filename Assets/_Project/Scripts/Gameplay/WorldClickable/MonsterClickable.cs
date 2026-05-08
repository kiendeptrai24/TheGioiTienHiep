using System;
using NUnit.Framework.Constraints;
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
            var playerNet = NetworkManager.SpawnManager.SpawnedObjects[heroId];
            if (playerNet == null) return;
            ulong networkOwner = playerNet.OwnerClientId;
            if (win)
            {
                RewardsAndPunishments(heroId, enemyId);
                NotifyResultClientRpc(
                $"Bạn đã thắng",
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { networkOwner }
                    }
                });
                SpawnMonter.Instance.RemoveNetObject(NetworkObject);
            }
            else
            {
                if (playerNet != null)
                {
                    playerNet.transform.position = new Vector3(500, 0, 440);
                    playerNet.transform.rotation = Quaternion.identity;
                }

                NotifyResultClientRpc(
                $"Bạn đã thua và sẽ được đưa tông môn",
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new[] { networkOwner }
                    }
                });
            }
        });
    }
    private void RewardsAndPunishments(ulong heroId, ulong enemyId)
    {
        if (!IsServer) return;

        var heroObject = NetworkManager.SpawnManager.SpawnedObjects[heroId];
        var enemyObject = NetworkManager.SpawnManager.SpawnedObjects[enemyId];

        if (heroObject == null || enemyObject == null) return;
        var heroResource = heroObject.GetComponent<ResourceStorage>();
        var enemyMapWorld = enemyObject.GetComponent<ItemMapWorld>();
        if (heroResource == null || enemyMapWorld == null) return;
        var itemData = enemyMapWorld.GetItemData() as DemonBeastData;
        if (itemData == null) return;
        ulong reward = itemData.lthach;
        heroResource.PlusCost(reward);
    }

    [ClientRpc]
    private void NotifyResultClientRpc(string message, ClientRpcParams clientRpcParams = default)
    {
        TopNotificationUI.Instance.ShowNotification(message);
    }

}
