using UnityEngine;

public class CloseButton : ActionButton
{
    [SerializeField] private UIPanelManager m_PanelManager;

    public override void OnClick()
    {
        m_PanelManager.HideMenuPanel();
    }
}
