using UnityEngine;

public class NavigationFindMapResult : ActionNavigation
{
    protected override void Awake()
    {
        base.Awake();
        m_ScreenName = "SearchMapResult";
    }
    public override void OnClick()
    {
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
}
