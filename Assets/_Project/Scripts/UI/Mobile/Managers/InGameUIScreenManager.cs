using UnityEngine;

public class InGameUIScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_VisualStandardScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_VisualStandardScreen.gameObject.name, m_VisualStandardScreen);
    }
    protected override void Start()
    {
        base.Start();
    }
}
