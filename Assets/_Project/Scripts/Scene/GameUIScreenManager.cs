using UnityEngine;

public class GameUIScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_InGameUI;
    [SerializeField] private GameObject m_UI;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_InGameUI.gameObject.name, m_InGameUI);
        m_Screens.Add(m_UI.gameObject.name, m_UI);
        defaultScreen = m_InGameUI.gameObject.name;
    }
    protected override void Start() 
    {
        base.Start();
    }
}
