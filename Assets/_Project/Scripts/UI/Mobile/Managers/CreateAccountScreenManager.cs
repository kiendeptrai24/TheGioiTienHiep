


using UnityEngine;

public class CreateAccountScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_LoginScreen;
    [SerializeField] private GameObject m_CreateNV1Screen;
    [SerializeField] private GameObject m_CreateNV2Screen;
    [SerializeField] private GameObject m_CreateNV3Screen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_LoginScreen.gameObject.name, m_LoginScreen);
        m_Screens.Add(m_CreateNV1Screen.gameObject.name, m_CreateNV1Screen);
        m_Screens.Add(m_CreateNV2Screen.gameObject.name, m_CreateNV2Screen);
        m_Screens.Add(m_CreateNV3Screen.gameObject.name, m_CreateNV3Screen);
        defaultScreen = m_CreateNV1Screen.gameObject.name;
    }


}