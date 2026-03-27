using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProfileDTO
{
    public string playerName;
    public string characterId;
    public ulong coins;

    // ===== OFFLINE MINING =====
    public MineOfflineDataList mineOfflineDataList = new MineOfflineDataList();  // Replaces Dictionary

    public PlayerProfileDTO()
    {
        mineOfflineDataList = new MineOfflineDataList();
    }
}
