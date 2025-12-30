using Unity.VisualScripting;
using UnityEngine;

public class CharacterScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_CharacterScreen;
    [SerializeField] private GameObject m_EquipmentScreen;
    [SerializeField] private GameObject m_StatsScreen;
    [SerializeField] private GameObject m_SkillsScreen;
    [SerializeField] private GameObject m_TechniqueScreen;
    [SerializeField] private GameObject m_TechniqueDetailScreen;
    [SerializeField] private GameObject m_InventoryScreen;
    [SerializeField] private GameObject m_ItemDetailScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_CharacterScreen.gameObject.name, m_CharacterScreen);
        m_Screens.Add(m_EquipmentScreen.gameObject.name, m_EquipmentScreen);
        m_Screens.Add(m_StatsScreen.gameObject.name, m_StatsScreen);
        m_Screens.Add(m_SkillsScreen.gameObject.name, m_SkillsScreen);
        m_Screens.Add(m_TechniqueScreen.gameObject.name, m_TechniqueScreen);
        m_Screens.Add(m_TechniqueDetailScreen.gameObject.name, m_TechniqueDetailScreen);
        m_Screens.Add(m_InventoryScreen.gameObject.name, m_InventoryScreen);
        m_Screens.Add(m_ItemDetailScreen.gameObject.name, m_ItemDetailScreen);
        defaultScreen = m_CharacterScreen.gameObject.name;
    }
    private void OnEnable() {
        StartUI(defaultScreen);
    }
}
