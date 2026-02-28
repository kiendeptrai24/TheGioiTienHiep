using UnityEngine;

public class ChatScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_ChatSmallPanelScreen;
    [SerializeField] private GameObject m_ChatLargePanelScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_ChatSmallPanelScreen.gameObject.name, m_ChatSmallPanelScreen);
        m_Screens.Add(m_ChatLargePanelScreen.gameObject.name, m_ChatLargePanelScreen);
        defaultScreen = m_ChatSmallPanelScreen.gameObject.name;
    }
    protected override void Start()
    {
        base.Start();
    }
}
