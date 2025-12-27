using UnityEngine;

public class CloseButton : ActionButton
{
    protected override void Awake() {
        base.Awake();
        m_ScreenManager = FindAnyObjectByType<GameUIScreenManager>();
    }

    public override void OnClick()
    {
        m_ScreenManager.NavigateTo("IngameUI");
    }
}
