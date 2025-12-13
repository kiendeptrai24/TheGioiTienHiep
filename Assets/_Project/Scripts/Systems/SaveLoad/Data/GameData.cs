using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public StatsCultivationPathData statsCultivationPathData;
    public StatsRaceData statsRaceData;
    public StatsRealmData statsRealmData;
    public List<ItemData> itemDatas;
    public GameData()
    {
        statsCultivationPathData = new StatsCultivationPathData();
        statsRaceData = new StatsRaceData();
        statsRealmData = new StatsRealmData();
        itemDatas = new List<ItemData>();
    }
}