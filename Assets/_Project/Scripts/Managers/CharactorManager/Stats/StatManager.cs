


using UnityEngine;

public class StatManager : Singleton<StatManager> 
{
    private StatsData statsData;
    protected override void Awake()
    {
        base.Awake();
        statsData = GetComponent<StatsData>();
    }
    public void SetStat(ItemData item)
    {
        statsData.SetUpItem(item);
    }
}