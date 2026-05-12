using UnityEngine;

public class SkillsScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_SkillsScreen;
    [SerializeField] private GameObject m_SkillDetailScreen;
    [SerializeField] private GameObject m_ItemDetailScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_SkillsScreen.gameObject.name, m_SkillsScreen);
        m_Screens.Add(m_SkillDetailScreen.gameObject.name, m_SkillDetailScreen);
        m_Screens.Add(m_ItemDetailScreen.gameObject.name, m_ItemDetailScreen);
        defaultScreen = m_SkillsScreen.gameObject.name;
    }
}
