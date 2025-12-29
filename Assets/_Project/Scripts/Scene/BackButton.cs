

using UnityEngine;

public class BackButton : ActionButton
{
    public override void OnClick()
    {
        screenManager.NavigateBack();
    }
}