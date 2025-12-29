using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncreasePointPopup : BasePopup<BaseSetupData, StatsPointPopupData>
{
    [SerializeField] private Button cancelBtn;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI attributeTxt;


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

    protected override StatsPointPopupData GetResult()
    {
        int value = 0;
        if (string.IsNullOrEmpty(inputField.text)) return null;
        try
        {
            value = int.Parse(inputField.text);
        }
        catch (System.Exception)
        {
            Debug.Log("IncreasePointPopup GetResult Error");
        }
        StatsPointPopupData data = new StatsPointPopupData(value);
        return data;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
    protected override void SetupPopupData(BaseSetupData data)
    {
        attributeTxt.text = data.Title;
    }
}