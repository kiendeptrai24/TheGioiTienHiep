using System;
using System.Collections.Generic;

public abstract class StatsModifierBase : IStatsModifier
{
    protected Dictionary<StatType, Stat> stats;
    public virtual void AddStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        this.stats = stats;
    }
    public virtual void RemoveStats(Dictionary<StatType, Stat> stats, ItemData itemData)
    {
        this.stats = stats;
    }
    protected void AddPercent(StatType type, float percent)
    {
        if (percent == 0) return;

        if (stats.TryGetValue(type, out Stat stat))
            stat.AddModifierPercent(percent);
    }
    protected void RemovePercent(StatType type, float percent)
    {
        if (percent == 0) return;

        if (stats.TryGetValue(type, out Stat stat))
            stat.RemoveModifierPercent(percent);
    }
    protected void AddValue(StatType type, float value)
    {
        if (value == 0) return;
        if (stats.TryGetValue(type, out Stat stat))
            stat.AddModifier(value);
    }
    protected void RemoveValue(StatType type, float value)
    {
        if (value == 0) return;
        if (stats.TryGetValue(type, out Stat stat))
            stat.RemoveModifier(value);
    }
}