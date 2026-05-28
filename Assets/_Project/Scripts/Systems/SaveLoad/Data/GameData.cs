using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string characterName;
    public string characterId;
    public Vector3 position;
    public Quaternion rotation;
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
        position = new Vector3(0, 0, 0);
        rotation = Quaternion.identity;
    }
    public void Clear()
    {
        characterName = "";
        characterId = "";
        coins = 0;
        potentialPoint = 0;
        skillPoint = 0;
        itemDatas.Clear();
        itemUsedDatas.Clear();
        itemInTeamDatas.Clear();
        itemShopDatas.Clear();
        allItemsDatas.Clear();
        gameBaseCharacterDatas.Clear();
        mineOfflineDataList.Clear();
    }

}