using UnityEngine;

public class NavigationToggle : ActionToggle
{
    [SerializeField] private string m_ScreenName;

    public override void OnClick(bool isOn)
    {
        if(m_ScreenManager.GetCurrentScreenName() != m_ScreenName)
            return;
        m_ScreenManager.GetCurrentScreen().SetActive(isOn);
    }
}
