
using UnityEngine;

public class NavigationButton : ActionButton
{
    [SerializeField]
    private string m_SceneName;

    public override void OnClick()
    {
        m_ScreenManager.NavigateTo(m_SceneName);
    }
}