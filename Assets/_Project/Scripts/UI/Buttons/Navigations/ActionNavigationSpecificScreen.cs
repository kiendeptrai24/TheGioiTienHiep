
using UnityEngine;

public class ActionNavigationSpecificScreen : ActionNavigation
{
    [Header("Specific Screen")]
    [SerializeField] private ScreenManager m_ScreenSpecific;
    [SerializeField] private string m_SpecificScreenName;

    public override void OnClick()
    {
        base.OnClick();
        m_ScreenManager.NavigateTo(m_ScreenName);
        m_ScreenSpecific.NavigateTo(m_SpecificScreenName);
    }
}