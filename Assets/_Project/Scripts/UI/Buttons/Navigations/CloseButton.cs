using UnityEngine;

public class CloseButton : ActionButton
{
    protected override void Awake()
    {
        base.Awake();
        screenManager = FindAnyObjectByType<GameUIScreenManager>();
    }

    public override void OnClick()
    {
        base.OnClick();
        screenManager.NavigateTo("IngameUI");
    }
}
