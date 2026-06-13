using System;
using Unity.Netcode;
using UnityEngine;

public class MineClickable : EntityClickable
{
    private SpiritStoneMine mines;
    private string mineId;
    private PlayerBattleRoster battleRoster;
    public override void OnNetworkSpawn()
    {
        mines = GetComponent<SpiritStoneMine>();
        entityWorldType = EntityWorldType.Mine;
        battleRoster = GetComponent<PlayerBattleRoster>();
        mineId = Guid.NewGuid().ToString();
    }

    public void UnLink(NetworkObject network)
    {
        UnlinkServerRpc(NetworkObjectId, network.NetworkObjectId);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void UnlinkServerRpc(ulong mineId, ulong ownerId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(mineId, out var mineObj))
            return;
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(ownerId, out var playerNet))
            return;

        var mineComponent = mineObj.GetComponent<SpiritStoneMine>();
        if (mineComponent == null) return;

        if (battleRoster != null)
            battleRoster.itemDatas = null;
        mineComponent.UnSetOwner(ownerId);
        NotifyResultClientRpc(
        $"Bạn đã bị mất liên kết với mỏ!",
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { playerNet.OwnerClientId }
            }
        });
    }
    public override void OnEntityClickedAccept(NetworkObject network)
    {
        EntityAcceptServerRpc(network.NetworkObjectId, NetworkObjectId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void EntityAcceptServerRpc(ulong ownerId, ulong mineId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(ownerId, out var ownerObj))
            return;
        if (mines == null) return;
        if (mines.ownership.HasOwner())
        {
            var playerNet = NetworkManager.SpawnManager.SpawnedObjects[ownerId];
            if (playerNet == null) return;
            BattleSimulatorRequest.Instance.RequestBattleSimulator(ownerId, mineId, (win) =>
            {
                if (win)
                {
                    SetOwner(ownerId);
                    NotifyResultClientRpc(
                    $"{TextColorUtil.Color("chiếm mỏ thành công", Color.green)} đang khai thác {TextColorUtil.Color("Mỏ linh thạch", Color.green)}!",
                    new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new[] { playerNet.OwnerClientId }
                        }
                    });
                }
                else
                {
                    var actorC = playerNet.GetComponent<ActorController>();
                    if (actorC != null)
                    {
                        Vector3 pos = new Vector3(500, 0, 440);
                        Vector3 scale = playerNet.transform.localScale;
                        Quaternion rot = Quaternion.identity;
                        actorC.TelePort(pos, rot, scale);
                    }
                    NotifyResultClientRpc(
                    $"{TextColorUtil.Color("Chiếm mỏ thất bại", Color.red)} sẽ được chuyển về {TextColorUtil.Color("TÔNG MÔN", Color.yellow)}!",
                    new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new[] { playerNet.OwnerClientId }
                        }
                    });
                }
            });
        }
        else
        {
            SetOwner(ownerId);
        }

    }
    private void SetOwner(ulong ownerId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(ownerId, out var playerNet))
            return;

        mines.SetOwner(ownerId);

        if (battleRoster == null) return;
        battleRoster.itemDatas = playerNet.GetComponent<PlayerBattleRoster>().itemDatas;

        NotifyResultClientRpc(
        $"Bạn đã chiếm được mở và bắt đầu khai thác!",
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { playerNet.OwnerClientId }
            }
        });
    }

    [ClientRpc]
    private void NotifyResultClientRpc(string message, ClientRpcParams clientRpcParams = default)
    {
        TopNotificationUI.Instance.ShowNotification(message);
    }
}
