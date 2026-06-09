


using UnityEngine;

public class StatManager : Singleton<StatManager>
{
    private StatsData statsData;
    [SerializeField] private HeroData championData;
    protected override void Awake()
    {
        base.Awake();
        statsData = GetComponent<StatsData>();
    }
    public void SetStat(ItemData item)
    {
        if(statsData == null) return;
        statsData.SetUpItem(item);
        if (item is HeroData heroData)
        {
            championData = heroData;
        }
    }
}