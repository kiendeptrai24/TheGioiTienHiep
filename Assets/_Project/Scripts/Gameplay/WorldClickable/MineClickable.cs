using System;
using Unity.Netcode;
using UnityEngine;

public class MineClickable : EntityClickable
{
    private SpiritStoneMine mine;
    private string mineId;

    private PlayerBattleRoster battleRoster;
    public override void OnNetworkSpawn()
    {
        mine = GetComponent<SpiritStoneMine>();
        entityWorldType = EntityWorldType.Mine;
        battleRoster = GetComponent<PlayerBattleRoster>();
        mineId = Guid.NewGuid().ToString();
    }

    public void UnLink(NetworkObject network)
    {
        UnlinkServerRpc(NetworkObjectId, network.NetworkObjectId);
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
        if (mine == null) return;
        if (mine.HasOwner())
        {
            var playerNet = NetworkManager.SpawnManager.SpawnedObjects[ownerId];
            if (playerNet == null) return;
            BattleSimulatorRequest.Instance.RequestBattleSimulator(ownerId, mineId, (win) =>
            {
                if (win)
                {
                    SetOwner(ownerId, ownerObj);
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
            SetOwner(ownerId, ownerObj);
        }

    }

    private void SetOwner(ulong ownerId, NetworkObject ownerObj)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(ownerId, out var playerNet))
            return;
        mine.SetOwner(ownerId,
        () =>
        {
            if (battleRoster == null) return;
            battleRoster.itemDatas = ownerObj.GetComponent<PlayerBattleRoster>().itemDatas;
            NotifyResultClientRpc(
            $"Bạn đã chiếm được mở và bắt đầu khai thác!",
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { playerNet.OwnerClientId }
                }
            });
        },
        (reason) =>
        {
            NotifyResultClientRpc(
            $"Không thể chiếm được mở! lí do: {reason}",
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { playerNet.OwnerClientId }
                }
            });
        });
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void UnlinkServerRpc(ulong mineId, ulong ownerId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(mineId, out var mineObj))
            return;
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(ownerId, out var playerNet))
            return;
        var mineComponent = mineObj.GetComponent<SpiritStoneMine>();
        if (mineComponent == null) return;
        Debug.Log("Unlink");
        NotifyResultClientRpc(
        $"Bạn đã bị mất liên kết với mỏ!",
        new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { playerNet.OwnerClientId }
            }
        });
        mineComponent.UnLink(ownerId);
    }


    public string GetMineId()
    {
        return mineId;
    }
    [ClientRpc]
    private void NotifyResultClientRpc(string message, ClientRpcParams clientRpcParams = default)
    {
        TopNotificationUI.Instance.ShowNotification(message);
    }
}
