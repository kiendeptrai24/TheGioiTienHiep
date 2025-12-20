

using UnityEngine;

public class BackButton : ActionButton
{
    public override void OnClick()
    {
        m_ScreenManager.NavigateBack();
    }
}