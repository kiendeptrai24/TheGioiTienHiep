

using UnityEngine;

public class ActionNavigationByName : ActionNavigation
{
    public override void OnClick()
    {
        base.OnClick();
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
}
