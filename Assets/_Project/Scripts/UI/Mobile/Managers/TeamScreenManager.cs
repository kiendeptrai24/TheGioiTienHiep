using UnityEngine;

public class TeamScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_TeamScreen;
    [SerializeField] private GameObject m_HerosScreen;
    [SerializeField] private GameObject m_HeroDetailScreen;
    [SerializeField] private GameObject m_heroStatsScreen;
    [SerializeField] private GameObject m_heroEquipScreen;
    [SerializeField] private GameObject m_itemDetailScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_TeamScreen.gameObject.name, m_TeamScreen);
        m_Screens.Add(m_HerosScreen.gameObject.name, m_HerosScreen);
        m_Screens.Add(m_HeroDetailScreen.gameObject.name, m_HeroDetailScreen);
        m_Screens.Add(m_heroStatsScreen.gameObject.name, m_heroStatsScreen);
        m_Screens.Add(m_heroEquipScreen.gameObject.name, m_heroEquipScreen);
        m_Screens.Add(m_itemDetailScreen.gameObject.name, m_itemDetailScreen);
        defaultScreen = m_TeamScreen.gameObject.name;
    }
    protected override void Start()
    {
        base.Start();
    }
}
