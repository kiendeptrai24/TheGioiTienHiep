
using UnityEngine;

public class NavigationButtonSpecificScreen : ActionButton
{
    [SerializeField] private string m_ScreenName;
    [Header("Specific Screen")]
    [SerializeField] private ScreenManager m_ScreenSpecific;
    [SerializeField] private string m_SpecificScreenName;

    public override void OnClick()
    {
        m_ScreenManager.NavigateTo(m_ScreenName);
        m_ScreenSpecific.NavigateTo(m_SpecificScreenName);
    
    }
}