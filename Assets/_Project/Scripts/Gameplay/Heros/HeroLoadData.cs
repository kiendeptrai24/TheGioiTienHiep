
using System;
using UnityEngine;

public class HeroLoadData : TGTHNetworkBehaviour, ISaveable
{
    [SerializeField] private string m_heroName;
    [SerializeField] private ItemData m_heroData;
    private StatsData stats;
    public event Action<HeroData> OnHeroDataLoaded;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
    }
    public void LoadData(GameData _data)
    {
        if (IsServer)
            return;
        foreach (var data in _data.itemDatas)
        {
            if (data.itemName == m_heroName && data is HeroData heroData)
            {
                m_heroData = heroData;
                OnHeroDataLoaded?.Invoke(m_heroData as HeroData);
                break;
            }
        }
        stats?.SetUpItem(m_heroData);
    }
    public void SaveGame(ref GameData _data)
    {

    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        stats = GetComponent<StatsData>();
    }
}