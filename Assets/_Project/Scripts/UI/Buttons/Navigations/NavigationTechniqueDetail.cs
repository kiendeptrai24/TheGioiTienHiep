using UnityEngine;

public class NavigationTechniqueDetail : ActionNavigation
{
    protected override void Awake()
    {
        base.Awake();
        m_ScreenName = "TechniqueDetail";
    }
    public override void OnClick()
    {
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
}
