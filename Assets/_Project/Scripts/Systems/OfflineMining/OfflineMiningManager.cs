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
    /// </summary>
    private void HandleOfflineCoinsOnLoad(GameData gameData)
    {
        if (gameData?.mineOfflineDataList == null || gameData.mineOfflineDataList.Count == 0)
        {
            Debug.Log("[OfflineMiningManager] No offline coins to process");
            return;
        }

        // Sum up all pending offline coins from all mines
        ulong totalOfflineCoins = 0;
        foreach (var mineData in gameData.mineOfflineDataList.mines)
        {
            totalOfflineCoins += mineData.accumulatedOfflineCoins;
        }

        if (totalOfflineCoins > 0)
        {
            // Add offline coins to player when server is ready
            StartCoroutine(WaitForNetworkAndAddOfflineCoins(totalOfflineCoins, gameData));
        }

        // Clear offline mining data
        gameData.mineOfflineDataList.Clear();
    }
    private IEnumerator WaitForNetworkAndAddOfflineCoins(ulong coins, GameData gameData)
    {
        // Wait for network to be ready and player to spawn
        yield return new WaitForSeconds(1f);

        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("[OfflineMiningManager] Server not ready, waiting...");
            yield return new WaitForSeconds(1f);
        }

        // Try to find player's ResourceStorage and add offline coins
        var players = FindObjectsByType<ResourceStorage>(FindObjectsSortMode.None);
        ResourceStorage playerStorage = null;

        foreach (var storage in players)
        {
            if (storage.GetComponent<NetworkObject>()?.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                playerStorage = storage;
                break;
            }
        }

        if (playerStorage != null)
        {
            playerStorage.AddOfflineCoins(coins);
            Debug.Log($"[OfflineMiningManager] Added {coins} offline coins to player!");
        }
        else
        {
            Debug.LogWarning("[OfflineMiningManager] Could not find player ResourceStorage");
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

    private void OnDestroy()
    {
        if (saveLoadManager != null)
            saveLoadManager.OnDataReadyToLoad -= HandleOfflineCoinsOnLoad;
    }
}
