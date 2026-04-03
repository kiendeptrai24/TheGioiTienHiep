

using System.Collections.Generic;
using UnityEngine;

public class BattleHistoryController : Singleton<BattleHistoryController> 
{
    public List<BattleHistory> battleEventsHistory;
    public void AddBattleHistory(BattleHistory battleEvents)
    {
        battleEventsHistory.Add(battleEvents);
    }
    public void ClearBattleHistory()
    {
        battleEventsHistory.Clear();
    }
}