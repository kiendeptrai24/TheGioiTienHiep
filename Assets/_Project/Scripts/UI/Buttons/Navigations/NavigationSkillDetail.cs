using UnityEngine;

public class NavigationSkillDetail : ActionNavigation
{
    protected override void Awake()
    {
        base.Awake();
        m_ScreenName = "SkillDetail";
    }
    public override void OnClick()
    {
        base.OnClick();
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
}
