

using UnityEngine;

public class NavigationButtonBack : ActionNavigation
{
    public override void OnClick()
    {
        m_ScreenManager.NavigateBack();
    }
}