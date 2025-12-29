using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UseItemPopup : BasePopup<BaseSetupData, BasePopupData>
{
    [SerializeField] private Button cancelBtn;
    [SerializeField] private TextMeshProUGUI description;


    public override void Show()
    {
        base.Show();
        //PopupAnimation.ShowPopup(rect, group, 0.5f);
    }
    protected override void SetupButtons()
    {
        base.SetupButtons();
        cancelBtn.onClick.AddListener(OnCancelClicked);
    }
    public override void Hide()
    {
        //PopupAnimation.HidePopup(rect, group, 0.5f);
        base.Hide();
    }

    protected override BasePopupData GetResult()
    {
        return null;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
    protected override void SetupPopupData(BaseSetupData data)
    {
        description.text = data.Title;
    }
}