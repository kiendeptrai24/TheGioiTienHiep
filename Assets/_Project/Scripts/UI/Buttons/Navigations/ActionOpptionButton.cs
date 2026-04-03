
using UnityEngine;
using UnityEngine.UI;

public class ActionOpptionButton : TGTHMonoBehaviour
{
    [SerializeField] private Button okeBtn;
    protected override void Awake()
    {
        base.Awake();
        okeBtn = GetComponent<Button>();
        okeBtn.onClick.AddListener(OnClickBtn);
    }

    private void OnClickBtn()
    {
        var itemData = InventoryCenterManager.Instance.playerCham;
        var popup = PopupManager.Instance.GetPopup<OpptionsPopup>();
        var data = new BaseSetupData();
        popup.ShowPopup(data);
    }

}