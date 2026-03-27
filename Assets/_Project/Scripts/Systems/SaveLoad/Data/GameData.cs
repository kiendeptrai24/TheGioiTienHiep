using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string characterName;
    public string characterId;
    public ulong coins;
    public List<ItemData> itemDatas;
    public List<ItemData> itemDatasInTeam;
    public List<ItemData> itemShopDatas;
    public List<ItemData> allItemsDatas;
    public List<ItemData> itemDatasCharacter;

    // ===== OFFLINE MINING STORAGE =====
    public MineOfflineDataList mineOfflineDataList = new MineOfflineDataList();  // Replaces Dictionary

    public GameData()
    {
        itemDatas = new List<ItemData>();
        itemDatasInTeam = new List<ItemData>();
        itemShopDatas = new List<ItemData>();
        allItemsDatas = new List<ItemData>();
        itemDatasCharacter = new List<ItemData>();
        mineOfflineDataList = new MineOfflineDataList();
    }
}