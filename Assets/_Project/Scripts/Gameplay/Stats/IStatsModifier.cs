using System;
using System.Collections.Generic;

public interface IStatsModifier
{
    public void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData);
    public void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData);
}