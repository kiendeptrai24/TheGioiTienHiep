using System;
using UnityEngine;

public class NavigationIngameUI : ActionNavigation
{
    protected override void Awake()
    {
        base.Awake();
        m_ScreenName = "IngameUI";
    }
    public override void OnClick()
    {
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
    public void SetScreenName(string name)
    {
        m_ScreenName = name;
    }
}

