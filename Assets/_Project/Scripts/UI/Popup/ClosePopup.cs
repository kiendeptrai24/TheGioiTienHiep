using UnityEngine;

public class ClosePopup : TGTHMonoBehaviour
{
    private PopupManager popupManager;
    protected override void Start()
    {
        base.Start();
        popupManager = PopupManager.Instance;
    }
    private void OnDisable()
    {
        if (popupManager != null)
            popupManager.HideAllPopups();
    }
}
