

using System;
using UnityEngine;

public class MenuSceneManager :  ScreenManager
{
    [Header("Home Screen")]
    [SerializeField] private GameObject m_MenuScreen;
    [Header("Character Screen")]
    [SerializeField] private GameObject m_CharacterScreen;
    [Header("Inventory Screen")]
    [SerializeField] private GameObject m_InventoryScreen;
    [SerializeField] private GameObject m_ItemDetailScreen;
    [SerializeField] private GameObject m_HeroesScreen;
    [SerializeField] private GameObject m_HeroeDetailScreen;
    [SerializeField] private GameObject m_ShopScreen;
    [SerializeField] private GameObject m_SettingScreen;
    [SerializeField] private GameObject m_HistoryScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_MenuScreen.gameObject.name, m_MenuScreen);
        m_Screens.Add(m_CharacterScreen.gameObject.name, m_CharacterScreen);
        // inventory
        m_Screens.Add(m_InventoryScreen.gameObject.name, m_InventoryScreen);
        m_Screens.Add(m_ItemDetailScreen.gameObject.name, m_ItemDetailScreen);
        
        m_Screens.Add(m_HeroesScreen.gameObject.name, m_HeroesScreen);
        m_Screens.Add(m_HeroeDetailScreen.gameObject.name, m_HeroeDetailScreen);
        m_Screens.Add(m_ShopScreen.gameObject.name, m_ShopScreen);
        // m_Screens.Add(MenuScreenType.Setting.ToString(), m_SettingScreen);
        // m_Screens.Add(MenuScreenType.History.ToString(), m_HistoryScreen);  
        defaultScreen = m_MenuScreen.gameObject.name;
    }
    protected override void Start() 
    {
        base.Start();
    }


}