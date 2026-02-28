


using UnityEngine;

public class MapScreenManager : ScreenManager 
{
    [SerializeField] private GameObject m_MapDetailScreen;
    [SerializeField] private GameObject m_SearchMapScreen;
    [SerializeField] private GameObject m_SearchMapDetailScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_MapDetailScreen.gameObject.name, m_MapDetailScreen);
        m_Screens.Add(m_SearchMapScreen.gameObject.name, m_SearchMapScreen);
        m_Screens.Add(m_SearchMapDetailScreen.gameObject.name, m_SearchMapDetailScreen);
        defaultScreen = m_MapDetailScreen.gameObject.name;
    }
    

}