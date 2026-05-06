using System;
using UnityEngine;


[Serializable]
public class PlayerProfileDTO
{
    public string playerName;
    public string characterId;
    public ulong coins;
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
[Serializable]
public struct Vector3DTO
{
    public float x;
    public float y;
    public float z;

    public Vector3DTO(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}