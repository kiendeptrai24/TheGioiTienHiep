using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages offline mining rewards when players reconnect.
/// When player logs in, this class:
/// 1. Checks if they had any pending offline coins from mines
/// 2. Calculates additional coins if mines accumulated while offline
/// 3. Adds pending coins to player's ResourceStorage
/// 4. Clears the offline mining data from PlayFab
/// </summary>
public class OfflineMiningManager : Singleton<OfflineMiningManager>
{
    [SerializeField] private SaveLoadPlayfab saveLoadManager;
    protected override void Awake()
    {
        base.Awake();
        if (saveLoadManager != null)
            saveLoadManager.OnDataReadyToLoad += HandleOfflineCoinsOnLoad;
    }


    /// <summary>
    /// Called when player data is loaded from PlayFab after reconnecting
    /// Triggers mine re-linking process
    /// </summary>
    private void HandleOfflineCoinsOnLoad(GameData gameData)
    {
        if (gameData?.mineOfflineDataList == null || gameData.mineOfflineDataList.Count == 0)
        {
            Debug.Log("[OfflineMiningManager] No offline data to process");
            return;
        }

        Debug.Log($"[OfflineMiningManager] Processing {gameData.mineOfflineDataList.Count} mines for reconnect");

        // Trigger mine re-linking on server
        StartCoroutine(ProcessMineRelinking(gameData));
    }

    /// <summary>
    /// Process mine re-linking when player reconnects
    /// Requests server to validate and re-link mines
    /// </summary>
    private IEnumerator ProcessMineRelinking(GameData gameData)
    {
        // Wait for network to be ready and player to spawn
        yield return new WaitUntil(() =>
            NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsConnectedClient &&
            NetworkManager.Singleton.LocalClient != null
        );

        yield return new WaitForSeconds(2f);  // Give time for scene setup

        // Find player's NetworkObject
        var playerNetObj = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (playerNetObj == null)
        {
            Debug.LogWarning("[OfflineMiningManager] Player object not found");
            yield break;
        }

        var playerStorage = playerNetObj.GetComponent<ResourceStorage>();
        if (playerStorage == null)
        {
            Debug.LogWarning("[OfflineMiningManager] ResourceStorage not found on player");
            yield break;
        }

        // Get list of mines to relink (stored in mineOfflineDataList)
        var minesToRelink = new List<ulong>();
        foreach (var mineData in gameData.mineOfflineDataList.mines)
        {
            // mineId is already ulong network object ID
            minesToRelink.Add(mineData.mineId);
        }

        if (minesToRelink.Count == 0)
        {
            Debug.Log("[OfflineMiningManager] No mines to relink");
            gameData.mineOfflineDataList.Clear();
            yield break;
        }

        // Request server to process mine relinking
        RequestMineRelinking(minesToRelink.ToArray(), gameData.characterId);

        // Clear offline data after requesting
        gameData.mineOfflineDataList.Clear();
    }

    /// <summary>
    /// Send RPC request to server to relink mines
    /// </summary>
    private void RequestMineRelinking(ulong[] mineNetworkIds, string playerId)
    {
        var playerMineRelinker = FindFirstObjectByType<PlayerMineRelinker>();
        if (playerMineRelinker != null)
        {
            Debug.Log($"[OfflineMiningManager] Requesting relink for {mineNetworkIds.Length} mines");
            playerMineRelinker.RequestMineRelinkServerRpc(mineNetworkIds, NetworkManager.Singleton.LocalClient.ClientId, playerId);
        }
        else
        {
            Debug.LogWarning("[OfflineMiningManager] PlayerMineRelinker not found in scene");
        }
    }

    /// <summary>
    /// Get total offline coins pending for a player
    /// </summary>
    public ulong GetTotalOfflineCoins(GameData gameData)
    {
        if (gameData?.mineOfflineDataList == null || gameData.mineOfflineDataList.Count == 0)
            return 0;

        ulong total = 0;
        foreach (var mineData in gameData.mineOfflineDataList.mines)
        {
            total += mineData.accumulatedOfflineCoins;
        }
        return total;
    }

    /// <summary>
    /// Get offline coins for a specific mine
    /// </summary>
    public ulong GetMineOfflineCoins(GameData gameData, ulong mineId)
    {
        if (gameData?.mineOfflineDataList == null)
            return 0;

        var mineData = gameData.mineOfflineDataList.GetMine(mineId);
        return mineData?.accumulatedOfflineCoins ?? 0;
    }

    /// <summary>
    /// Add offline coins to a specific mine (called when mine's owner is offline)
    /// </summary>
    public void AddMineOfflineCoins(GameData gameData, ulong mineId, ulong amount, string owner)
    {
        if (gameData?.mineOfflineDataList == null)
            return;

        double currentTime = NetworkManager.Singleton != null ? NetworkManager.Singleton.ServerTime.Time : 0;

        var existing = gameData.mineOfflineDataList.GetMine(mineId);
        if (existing != null)
        {
            existing.accumulatedOfflineCoins += amount;
        }
        else
        {
            gameData.mineOfflineDataList.mines.Add(
                new MineOfflineData(mineId, amount, currentTime, owner)
            );
        }
    }

    /// <summary>
    /// Clear offline coins for a specific mine
    /// </summary>
    public void ClearMineOfflineCoins(GameData gameData, ulong mineId)
    {
        if (gameData?.mineOfflineDataList == null)
            return;

        gameData.mineOfflineDataList.Remove(mineId);
    }
}
