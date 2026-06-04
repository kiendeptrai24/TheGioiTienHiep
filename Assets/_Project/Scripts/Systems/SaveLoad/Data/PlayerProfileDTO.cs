using System;
using UnityEngine;


[Serializable]
public class PlayerProfileDTO
{
    public string playerName;
    public string characterId;
    public ulong coins;
    public int currentHealth;
    public string createdAt;
    public Vector3DTO position;
    public Vector3DTO rotation;
    public int potentialPoint;
    public int skillPoint;
    // ===== OFFLINE MINING =====
    public MineOfflineDataList mineOfflineDataList = new MineOfflineDataList();  // Replaces Dictionary

    public PlayerProfileDTO()
    {
        mineOfflineDataList = new MineOfflineDataList();
    }
}
