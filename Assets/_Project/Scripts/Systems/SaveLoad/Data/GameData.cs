using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string characterName;
    public string characterId;
    public string createdAt;
    public Vector3 position;
    public Quaternion rotation;
    public ulong coins;
    public int potentialPoint;
    public int skillPoint;
    public ItemDataPoint itemDataPoint;
    public List<ItemData> itemDatas;
    public List<ItemData> itemUsedDatas; // champion 
    public List<ItemData> itemChampionInTeamDatas; // champion in team
    public List<ItemData> itemShopDatas; // item shop
    public List<ItemData> itemCharacterDatas; // item character you have
    public List<ItemData> gameBaseCharacterDatas; // item character base
    public MineOfflineDataList mineOfflineDataList;

    public GameData()
    {
        itemDatas = new List<ItemData>();
        itemUsedDatas = new List<ItemData>();
        itemChampionInTeamDatas = new List<ItemData>();
        itemShopDatas = new List<ItemData>();
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
        itemChampionInTeamDatas.Clear();
        itemShopDatas.Clear();
        gameBaseCharacterDatas.Clear();
        mineOfflineDataList.Clear();
    }

}