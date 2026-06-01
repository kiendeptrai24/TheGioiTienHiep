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
        BattleSimulatorRequest.Instance.RequestBattleSimulator(heroId, enemyId, (win) =>
        {
            var playerNet = NetworkManager.SpawnManager.SpawnedObjects[heroId];
            if (playerNet == null) return;
            ulong networkOwner = playerNet.OwnerClientId;
            if (win)
            {
                RewardsAndPunishments(heroId, enemyId);
                SpawnMonster.Instance.RemoveNetObject(NetworkObject);
            }
            else
            {
                if (playerNet != null)
                {
                    var actorC = playerNet.GetComponent<ActorController>();
                    if (actorC != null)
                    {
                        Vector3 pos = new Vector3(500, 0, 440);
                        Vector3 scale = playerNet.transform.localScale;
                        Quaternion rot = Quaternion.identity;
                        actorC.TelePort(pos, rot, scale);
                    }
                }

                NotifyResultClientRpc(
                $"{TextColorUtil.Color("Bạn đã thua", Color.red)} sẽ được chuyển về {TextColorUtil.Color("TÔNG MÔN", Color.yellow)}!",
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
        var enemyMapWorld = enemyObject.GetComponent<ResourceNode>();
        if (heroResource == null || enemyMapWorld == null) return;
        var itemData = enemyMapWorld.GetData() as DemonBeastData;
        if (itemData == null) return;
        ulong reward = itemData.lthach;
        heroResource.PlusCost(reward);
        NotifyResultClientRpc(
        $"{TextColorUtil.Color("Chiến thắng", Color.green)} Nhận được {TextColorUtil.Color(reward.ToString(), Color.green)} Linh Thạch",
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { heroObject.OwnerClientId }
            }
        });
    }

    [ClientRpc]
    private void NotifyResultClientRpc(string message, ClientRpcParams clientRpcParams = default)
    {
        TopNotificationUI.Instance.ShowNotification(message);
    }

}
