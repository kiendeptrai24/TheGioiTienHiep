using UnityEngine;

public class GameUIScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_InGameUI;
    [SerializeField] private GameObject m_UI;
    [SerializeField] private GameObject m_MapScreen;
    [SerializeField] private GameObject m_TeamScreen;
    [SerializeField] private GameObject m_CreateAccountScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_InGameUI.gameObject.name, m_InGameUI);
        m_Screens.Add(m_UI.gameObject.name, m_UI);
        m_Screens.Add(m_MapScreen.gameObject.name, m_MapScreen);
        m_Screens.Add(m_TeamScreen.gameObject.name, m_TeamScreen);
        m_Screens.Add(m_CreateAccountScreen.gameObject.name, m_CreateAccountScreen);
        defaultScreen = m_CreateAccountScreen.gameObject.name;
    }
    protected override void Start()
    {
        base.Start();
    }
}
