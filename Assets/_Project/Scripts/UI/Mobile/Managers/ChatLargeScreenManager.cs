using System.Collections.Generic;
using UnityEngine;

public class ChatLargeScreenManager : ScreenManager
{
    [Header("Chat Button")]
    [SerializeField] private GameObject m_GeneralbtnScreen;
    [SerializeField] private GameObject m_ChatPrivatebtnScreen;
    [Header("Chat Panel")]
    [SerializeField] private GameObject m_GeneralPanelScreen;
    [SerializeField] private GameObject m_ChatPrivatePanelScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_GeneralPanelScreen.gameObject.name, m_GeneralPanelScreen);
        m_ScreensList.Add(m_GeneralPanelScreen.gameObject.name, new List<GameObject> { m_GeneralbtnScreen });

        m_Screens.Add(m_ChatPrivatePanelScreen.gameObject.name, m_ChatPrivatePanelScreen);
        m_ScreensList.Add(m_ChatPrivatePanelScreen.gameObject.name, new List<GameObject> { m_ChatPrivatebtnScreen });

        defaultScreen = m_GeneralPanelScreen.gameObject.name;
    }
    protected override void Start()
    {
        base.Start();
    }
}
