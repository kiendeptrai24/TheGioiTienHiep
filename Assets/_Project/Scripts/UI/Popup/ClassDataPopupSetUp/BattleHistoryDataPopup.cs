using System.Collections.Generic;
using UnityEngine;

public class BattleHistoryDataPopup : IPopupData
{
    public List<BattleHistory> battleHistories;
    public BattleHistoryDataPopup(List<BattleHistory> battleHistory)
    {
        this.battleHistories = battleHistory;
    }
    public BattleHistoryDataPopup() { }
}