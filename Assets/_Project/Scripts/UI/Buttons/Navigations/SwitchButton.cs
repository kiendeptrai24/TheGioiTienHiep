

using UnityEngine;

public class SwitchButton : ActionButton
{
    [SerializeField] private string m_ScreenName;

    public override void OnClick()
    {
        base.OnClick();
        screenManager.SwitchTo(m_ScreenName);
    }
}