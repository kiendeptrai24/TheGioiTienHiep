using Unity.Netcode;
using UnityEngine;

public class MineClickable : EntityClickable
{
    private SpiritStoneMine mine;
    public override void OnNetworkSpawn()
    {
        mine = GetComponent<SpiritStoneMine>();
        entityWorldType = EntityWorldType.Mine;
    }
    public bool IsObjectOwner(NetworkObject owner)
    {
        return mine.IsObjectOwner(owner);
    }
    public void UnLink(NetworkObject network)
    {
        UnlinkServerRpc(NetworkObjectId, network.NetworkObjectId);
    }
    public override void OnEntityClickedAccept(NetworkObject network)
    {
        EntityAcceptServerRpc(NetworkObjectId, network.NetworkObjectId);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void EntityAcceptServerRpc(ulong mineId, ulong ownerId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(mineId, out var mineObj))
            return;
        var mineComponent = mineObj.GetComponent<SpiritStoneMine>();
        if (mineComponent == null) return;
        mineComponent.SetOwner(ownerId);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void UnlinkServerRpc(ulong mineId, ulong ownerId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(mineId, out var mineObj))
            return;

        var mineComponent = mineObj.GetComponent<SpiritStoneMine>();
        if (mineComponent == null) return;

        mineComponent.UnLink(ownerId);
    }
}
