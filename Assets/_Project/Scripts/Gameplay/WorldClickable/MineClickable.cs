using Unity.Netcode;
using UnityEngine;

public class MineClickable : EntityClickable
{
    private SpiritStoneMine mine;
    private string mineId;  // ===== NEW: Track mine identity
    private PlayerBattleRoster battleRoster;
    public override void OnNetworkSpawn()
    {
        mine = GetComponent<SpiritStoneMine>();
        entityWorldType = EntityWorldType.Mine;
        battleRoster = GetComponent<PlayerBattleRoster>();
        mineId = $"mine_{NetworkObjectId}";
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
            BattleSimulatorRequest.Instance.RequestBattleSimulator(ownerId, mineId, (win) =>
            {
                if (win)
                {
                    SetOwner(ownerId, ownerObj);
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
        mine.SetOwner(ownerId,
        () =>
        {
            if (battleRoster == null) return;
            battleRoster.itemDatas = ownerObj.GetComponent<PlayerBattleRoster>().itemDatas;
        },
        (reason) =>
        {
            Debug.Log("Fail: " + reason);
        });
        UpdateMineOwnershipInGameData(ownerId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void UnlinkServerRpc(ulong mineId, ulong ownerId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(mineId, out var mineObj))
            return;

        var mineComponent = mineObj.GetComponent<SpiritStoneMine>();
        if (mineComponent == null) return;

        // ===== NEW: Clear mine ownership before unlink =====
        mineComponent.UnLink(ownerId);
        ClearMineOwnershipFromGameData();
    }

    // ===== HELPER: Update mine ownership in GameData =====
    private void UpdateMineOwnershipInGameData(ulong ownerId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        // This would need to be called on a GameData manager
        // For now, it's handled through PlayFab ProfileService
        Debug.Log($"[MineClickable] Mine {mineId} claimed by player {ownerId}");
    }

    // ===== HELPER: Clear mine ownership =====
    private void ClearMineOwnershipFromGameData()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        Debug.Log($"[MineClickable] Mine {mineId} unlinked");
    }

    public string GetMineId()
    {
        return mineId;
    }

    public SpiritStoneMine GetMine()
    {
        return mine;
    }
}
