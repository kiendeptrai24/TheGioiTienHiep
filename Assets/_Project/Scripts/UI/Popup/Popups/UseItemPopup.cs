using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UseItemPopup : BasePopup<BaseSetupData, BasePopupData>
{
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button infoBtn;
    [SerializeField] private TextMeshProUGUI description;

    protected Action onInfo;
    public override void Show()
    {
        base.Show();
        //PopupAnimation.ShowPopup(rect, group, 0.5f);
    }
    protected override void SetupButtons()
    {
        base.SetupButtons();
        cancelBtn.onClick.AddListener(() => { 
            m_EffectManager?.PlayOneShot("button-click");
            OnCancelClicked(); });
        infoBtn.onClick.AddListener(() => { 
            m_EffectManager?.PlayOneShot("button-click");
            OnInfoClicked(); });
    }

    private void OnInfoClicked()
    {
        onInfo?.Invoke();
        PopupManager.Instance.HidePopup(this);
    }
    public void ShowPopup(BaseSetupData data, Action<BasePopupData> onConfirm = null,
     Action onCancel = null, Action onInfo = null)
    {
        base.ShowPopup(data, onConfirm, onCancel);
        this.onCancel = onCancel;
        this.onInfo = onInfo;
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