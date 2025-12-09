using System.Collections.Generic;

public interface IStatProvider
{
    void ApplyStats(Dictionary<StatType, Stat> stats);
}