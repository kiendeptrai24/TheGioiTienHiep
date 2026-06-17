
using UnityEngine;
using UnityEngine.UI;

public class ActionOpptionButton : TGTHMonoBehaviour
{
    [SerializeField] private Button okeBtn;
    private OpptionsPopup popup;
    protected override void Awake()
    {
        base.Awake();
        okeBtn = GetComponent<Button>();
        okeBtn.onClick.AddListener(OnClickBtn);
        popup = PopupManager.Instance.GetPopup<OpptionsPopup>();
    }
    protected override void Start()
    {
        base.Start();

    }
    private void OnClickBtn()
    {
        var data = new AudioSetupData();
        popup.ShowPopup(data);
    }

}