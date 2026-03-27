

using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class ItemResourseData : ItemData
{
    [JsonIgnore]
    public ResourceType resourceType;
    [JsonIgnore]
    public Vector3 position;
    [JsonIgnore]
    public int yieldPerHarvest;

    [JsonIgnore]
    public float miningTime;
    [JsonIgnore]
    public float currentMiningProgress;
    [JsonIgnore]
    public int maxStorage;
    [JsonIgnore]
    public int currentAmount;
    [JsonIgnore]
    public int level;

    // ===== OFFLINE MINING FIELDS =====
    [JsonIgnore]
    public ulong accumulatedOfflineCoins;  // Coins accumulated while owner offline
    [JsonIgnore]
    public double lastOwnerClaimTime;      // Server time when owner last received coins
}