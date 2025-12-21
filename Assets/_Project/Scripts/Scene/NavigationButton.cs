
using UnityEngine;

public class NavigationButton : ActionButton
{
    public enum ActionButtonMode
    {
        General,
        SwitchScreen
    }
    [SerializeField] private ActionButtonMode actionMode;
    [SerializeField] private string m_ScreenName;

    public override void OnClick()
    {
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
}