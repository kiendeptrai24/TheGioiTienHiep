using Unity.VisualScripting;
using UnityEngine;

public class HeroScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_HeroScreen;
    [SerializeField] private GameObject m_HeroEquipmentScreen;
    [SerializeField] private GameObject m_HeroDetailScreen;
    [SerializeField] private GameObject m_HeroStatsScreen;
    [SerializeField] private GameObject m_ItemDetailScreen;
    [SerializeField] private GameObject m_SkillsScreen;
    [SerializeField] private GameObject m_TechniqueDetailScreen;
    [SerializeField] private GameObject m_InventoryScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_HeroScreen.gameObject.name, m_HeroScreen);
        m_Screens.Add(m_HeroEquipmentScreen.gameObject.name, m_HeroEquipmentScreen);
        m_Screens.Add(m_HeroStatsScreen.gameObject.name, m_HeroStatsScreen);
        m_Screens.Add(m_HeroDetailScreen.gameObject.name, m_HeroDetailScreen);
        m_Screens.Add(m_ItemDetailScreen.gameObject.name, m_ItemDetailScreen);

        defaultScreen = m_HeroScreen.gameObject.name;
    }
    private void OnEnable() {
        StartUI(defaultScreen);
    }
}
