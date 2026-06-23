using System;
using UnityEngine;

public class NavigationCharactorSelection : ActionNavigation
{
    protected override void Awake()
    {
        base.Awake();
        m_ScreenName = "Panel (CreateNv)";
    }
    public override void OnClick()
    {
        base.OnClick();
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
    public void SetScreenName(string name)
    {
        m_ScreenName = name;
    }
}

