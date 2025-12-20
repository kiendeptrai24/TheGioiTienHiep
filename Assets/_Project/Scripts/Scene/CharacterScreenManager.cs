using UnityEngine;

public class CharacterScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_CharacterScreen;
    [SerializeField] private GameObject m_StatsScreen;
    [SerializeField] private GameObject m_SkillsScreen;
    [SerializeField] private GameObject m_TechniqueScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_CharacterScreen.gameObject.name, m_CharacterScreen);
        m_Screens.Add(m_StatsScreen.gameObject.name, m_StatsScreen);
        m_Screens.Add(m_SkillsScreen.gameObject.name, m_SkillsScreen);
        m_Screens.Add(m_TechniqueScreen.gameObject.name, m_TechniqueScreen);
        defaultScreen = m_CharacterScreen.gameObject.name;
    }
    protected override void Start() 
    {
        base.Start();
    }
}
