

using UnityEngine;

public class NavigationItemDetail : ActionNavigation
{
    protected override void Awake()
    {
        base.Awake();
        m_ScreenName = "ItemDetail";
    }
    public override void OnClick()
    {
        Debug.Log("OnClick");
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
}