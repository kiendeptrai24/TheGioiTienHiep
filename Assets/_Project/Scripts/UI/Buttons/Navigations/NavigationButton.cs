
using UnityEngine;

public class NavigationButton : ActionButton
{
    [SerializeField] private string m_ScreenName;

    public override void OnClick()
    {
        screenManager.NavigateTo(m_ScreenName);
    }
}