
using System;
using UnityEngine;

public class HeroLoadData : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private bool loadHeroData = false;
    [SerializeField] private string m_heroName;
    [SerializeField] private ItemData m_heroData;

    [SerializeField] private HeroPreset heroPreset;
    public event Action<HeroData> OnHeroDataLoaded;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
        m_heroData = heroPreset.GetItemData();
        OnHeroDataLoaded?.Invoke(m_heroData as HeroData);
    }
    public void LoadData(GameData _data)
    {
        // if (loadHeroData) return;
        // foreach (var data in _data.itemDatas)
        // {
        //     if (data.itemName == m_heroName && data is HeroData heroData)
        //     {
        //         m_heroData = heroData;
        //         OnHeroDataLoaded?.Invoke(m_heroData as HeroData);
        //         break;
        //     }
        // }
    }
    public void SaveGame(ref GameData _data)
    {

    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}