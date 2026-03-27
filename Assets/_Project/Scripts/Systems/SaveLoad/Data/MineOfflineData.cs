using System;
using System.Collections.Generic;

/// <summary>
/// Serializable wrapper for a single mine's offline mining data
/// Use this instead of Dictionary for PlayFab compatibility
/// </summary>
[Serializable]
public class MineOfflineData
{
    public ulong mineId;                  // Unique mine identifier
    public ulong accumulatedOfflineCoins;  // Coins accumulated while owner offline
    public double lastClaimTime;           // Server time when owner last claimed coins
    public string playerId;            // Character ID of current owner
    
    public MineOfflineData() { }
    
    public MineOfflineData(ulong mineId, ulong coins = 0, double lastTime = 0, string playerId = "")
    {
        this.mineId = mineId;
        this.accumulatedOfflineCoins = coins;
        this.lastClaimTime = lastTime;
        this.playerId = playerId;
    }
    
    public override string ToString()
    {
        return $"[Mine: {mineId}, Coins: {accumulatedOfflineCoins}, LastClaim: {lastClaimTime}, Owner: {playerId}]";
    }
}

/// <summary>
/// List wrapper for serialization - replaces Dictionary usage
/// </summary>
[Serializable]
public class MineOfflineDataList
{
    public List<MineOfflineData> mines = new List<MineOfflineData>();
    
    public MineOfflineData GetMine(ulong mineId)
    {
        foreach (var mine in mines)
        {
            if (mine.mineId == mineId)
                return mine;
        }
        return null;
    }
    
    public void AddOrUpdate(ulong mineId, ulong coins, double lastTime, string playerId)
    {
        var existing = GetMine(mineId);
        if (existing != null)
        {
            existing.accumulatedOfflineCoins = coins;
            existing.lastClaimTime = lastTime;
            existing.playerId = playerId;
        }
        else
        {
            mines.Add(new MineOfflineData(mineId, coins, lastTime, playerId));
        }
    }
    
    public void Remove(ulong mineId)
    {
        mines.RemoveAll(m => m.mineId == mineId);
    }
    
    public void Clear()
    {
        mines.Clear();
    }
    
    public int Count => mines.Count;
}
