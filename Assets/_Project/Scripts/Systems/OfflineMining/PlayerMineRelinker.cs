using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Server-side Manager for re-linking and validating player mines on reconnect
/// 
/// FLOW:
/// 1. Player connects → PlayFab data loaded
/// 2. Client sends RelinkMinesRequest (via RPC)
/// 3. Server checks each mine:
///    - EXISTS & UNCLAIMED: Re-link + add pending coins + continue mining
///    - EXISTS & CLAIMED BY OTHER: Add stolen coins + don't re-link
///    - NOT EXISTS: Ignore
/// 4. Server sends back RelinkData (linked mines, pending coins)
/// 5. Client updates UI, continue playing
/// </summary>
public class PlayerMineRelinker : TGTHNetworkBehaviour
{
    [Serializable]
    public class MineRelinkData
    {
        public ulong mineId;
        public bool relinked;              // Was re-linked
        public ulong pendingCoins;         // Offline coins to add
        public string stolenByPlayerId;    // Who stole it (if applicable)
    }

    public event Action<List<MineRelinkData>> OnMinesRelinked;
    public event Action<List<ulong>> OnResourceIdsChanged;

    [SerializeField] private List<ulong> _resourceIds = new List<ulong>();
    [ClientRpc]
    private void SyncResourceIdsClientRpc(ulong[] resourceIds)
    {
        _resourceIds = resourceIds.ToList();
        OnResourceIdsChanged?.Invoke(_resourceIds);
    }
    public void AddResource(ulong netId)
    {
        if (!IsServer) return;

        _resourceIds.Add(netId);
        SyncResourceIdsClientRpc(_resourceIds.ToArray());
    }

    public void RemoveResource(ulong netId)
    {
        if (!IsServer) return;

        if (_resourceIds.Contains(netId))
        {
            _resourceIds.Remove(netId);
            SyncResourceIdsClientRpc(_resourceIds.ToArray());
        }

    }

    /// <summary>
    /// Client calls this RPC when reconnecting to re-link their mines
    /// </summary>
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void RequestMineRelinkServerRpc(ulong[] mineNetworkObjectIds, ulong clientId, string playerId)
    {
        if (!IsServer)
            return;

        var playerObject = NetworkManager.Singleton
            .ConnectedClients[clientId]
            .PlayerObject;

        if (playerObject == null)
        {
            Debug.LogError("[PlayerMineRelinker] Player object not found");
            return;
        }

        var relinkResults = new List<MineRelinkData>();

        // Process each mine the player was mining
        foreach (var mineNetId in mineNetworkObjectIds)
        {
            if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(mineNetId, out var mineNetObj))
            {
                Debug.LogWarning($"[PlayerMineRelinker] Mine {mineNetId} no longer exists");
                continue;
            }

            var spiritMine = mineNetObj.GetComponent<SpiritStoneMine>();
            if (spiritMine == null)
                continue;

            var relinkData = ProcessMineRelink(spiritMine, playerObject, clientId, playerId);
            relinkResults.Add(relinkData);
        }

        // Notify client of results
        OnMinesRelinked?.Invoke(relinkResults);
        Debug.Log($"[PlayerMineRelinker] Processed {relinkResults.Count} mines for player {clientId}");
    }

    /// <summary>
    /// Process single mine relink logic
    /// </summary>
    private MineRelinkData ProcessMineRelink(SpiritStoneMine mine, NetworkObject playerObject, ulong clientId, string playerId)
    {
        var data = new MineRelinkData();

        data.mineId = mine.NetworkObjectId;

        // Check if mine is currently owned
        Debug.Log(playerId);
        bool isOwner = mine.PlayerIsOwner(playerId);
        if (isOwner)
        {
            data.pendingCoins = mine.GetPendingOfflineCoins();
            data.relinked = true;
            data.stolenByPlayerId = null;
            mine.AddOfflineCoinsToOwner(playerObject.NetworkObjectId, playerId);
            Debug.Log("GetPendingOfflineCoins");
        }
        else
        {
            data.relinked = false;
            data.pendingCoins = 0;
        }

        return data;
    }

    /// <summary>
    /// Calculate coins accumulated while player was offline
    /// Based on the last claim time in the mine data
    /// </summary>
    private ulong CalculateStolenCoins(ItemResourseData mineData)
    {
        if (mineData == null)
            return 0;

        double offlineDuration = NetworkManager.ServerTime.Time - mineData.lastOwnerClaimTime;
        if (offlineDuration <= 0)
            return 0;

        float yieldPerSecond = mineData.yieldPerHarvest / mineData.miningTime;
        ulong stolenCoins = (ulong)(yieldPerSecond * offlineDuration);

        Debug.Log($"[PlayerMineRelinker] Calculated stolen coins: {stolenCoins} (duration: {offlineDuration}s, yield/sec: {yieldPerSecond})");
        return stolenCoins;
    }
    /// <summary>
    /// Get all active mines in the world
    /// </summary>
    public static List<SpiritStoneMine> GetAllActiveMines()
    {
        var mines = new List<SpiritStoneMine>();
        var mineObjects = FindObjectsByType<SpiritStoneMine>(FindObjectsSortMode.None);
        foreach (var mine in mineObjects)
        {
            if (mine.gameObject.activeInHierarchy)
                mines.Add(mine);
        }
        return mines;
    }
}
