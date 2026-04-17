using System;
using UnityEngine;

public class RootGameUIScreenManager : ScreenManager
{
    [SerializeField] private GameObject m_UI;
    [SerializeField] private GameObject m_UIBG;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_UI.gameObject.name, m_UI);
        m_Screens.Add(m_UIBG.gameObject.name, m_UIBG);
        defaultScreen = m_UI.gameObject.name;
        if (Configuration.Instance.buildType == BuildType.LOCAL_SERVER ||
         Configuration.Instance.buildType == BuildType.REMOTE_SERVER)
        {
            HideAll();
        }
    }
    protected override void Start()
    {
        base.Start();
        if (Configuration.Instance.buildType == BuildType.LOCAL_SERVER ||
         Configuration.Instance.buildType == BuildType.REMOTE_SERVER)
            return;
        BattlePlaybackManager.Instance.OnReadyGame += OnReadyGame;
        BattlePlaybackManager.Instance.OnEndGame += OnEndGame;
    }

    private void OnEndGame()
    {
        NavigateTo(m_UI.gameObject.name);
    }

    private void OnReadyGame()
    {
        NavigateTo(m_UIBG.gameObject.name);
    }
}
