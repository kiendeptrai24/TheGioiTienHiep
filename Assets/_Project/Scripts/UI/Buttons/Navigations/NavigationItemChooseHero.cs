

public class NavigationItemChooseHero : ActionNavigation
{
    protected override void Awake()
    {
        base.Awake();
        m_ScreenName = "ChooseHero";
    }
    public override void OnClick()
    {
        m_ScreenManager.NavigateTo(m_ScreenName);
    }
}