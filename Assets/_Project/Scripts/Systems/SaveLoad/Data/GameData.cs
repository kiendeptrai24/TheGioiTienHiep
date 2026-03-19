using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string playerName;
    public ulong coins;
    public List<ItemData> itemDatas;
    public List<ItemData> itemDatasInTeam;
    public List<ItemData> itemDatasExisting;
    public List<ItemData> itemShopDatas;
    public List<ItemData> allItemsDatas;
    public GameData()
    {
        itemDatas = new List<ItemData>();
        itemDatasInTeam = new List<ItemData>();
        itemDatasExisting = new List<ItemData>();
        itemShopDatas = new List<ItemData>();
        allItemsDatas = new List<ItemData>();
    }
}