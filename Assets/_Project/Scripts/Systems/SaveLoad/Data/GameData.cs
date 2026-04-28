using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string characterName;
    public Vector3 position;
    public Quaternion rotation;
    public string characterId;
    public ulong coins;
    public int potentialPoint;
    public int skillPoint;
    public ItemDataPoint itemDataPoint;
    public List<ItemData> itemDatas;
    public List<ItemData> itemUsedDatas;
    public List<ItemData> itemInTeamDatas;
    public List<ItemData> itemShopDatas;
    public List<ItemData> allItemsDatas;
    public List<ItemData> itemCharacterDatas;
    public List<ItemData> gameBaseCharacterDatas;
    public MineOfflineDataList mineOfflineDataList;

    public GameData()
    {
        itemDatas = new List<ItemData>();
        itemUsedDatas = new List<ItemData>();
        itemInTeamDatas = new List<ItemData>();
        itemShopDatas = new List<ItemData>();
        allItemsDatas = new List<ItemData>();
        itemCharacterDatas = new List<ItemData>();
        gameBaseCharacterDatas = new List<ItemData>();
        mineOfflineDataList = new MineOfflineDataList();
        itemDataPoint = new ItemDataPoint();
        position = new Vector3(0, 0, 0);
        rotation = Quaternion.identity;
    }
}