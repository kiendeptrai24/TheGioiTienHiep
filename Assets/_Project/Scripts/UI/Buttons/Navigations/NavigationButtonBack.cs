

using UnityEngine;

public class NavigationButtonBack : ActionNavigation
{
    public override void OnClick()
    {
        base.OnClick();
        m_ScreenManager.NavigateBack();
    }
}