

using UnityEngine;

public class BackButton : ActionButton
{
    public override void OnClick()
    {
        base.OnClick();
        screenManager.NavigateBack();
    }
}