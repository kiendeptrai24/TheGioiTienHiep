using System;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackPopup : BasePopup<BaseSetupData, FeedbackPopupData>
{
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI userIdTxt;
    [SerializeField] private TMP_InputField titleInputField;
    [SerializeField] private TMP_InputField feedbackInputField;
    private ProfileManager profileManager;
    private string userId;
    protected override void Awake()
    {
        base.Awake();
        GetUserId();
    }

    private string GetUserId()
    {
        profileManager = ProfileManager.Instance;
        var currentProfile = profileManager.GetProfile();
        userId = currentProfile.userId;
        return userId;
    }

    public override void Show()
    {
        base.Show();
    }
    protected override void SetupButtons()
    {
        base.SetupButtons();
        cancelBtn.onClick.AddListener(() => { 
            m_EffectManager?.PlayOneShot("button-click");
            OnCancelClicked(); });
        closeBtn.onClick.AddListener(() => { 
            m_EffectManager?.PlayOneShot("button-click");
            OnCancelClicked(); });
    }
    public override void Hide()
    {
        base.Hide();
    }
    protected override bool ValidateResult(FeedbackPopupData result)
    {
        if (string.IsNullOrEmpty(result.title) || string.IsNullOrEmpty(result.message))
        {
            return false;
        }
        return true;
    }
    protected override FeedbackPopupData GetResult()
    {
        var feedbackData = new FeedbackPopupData(userId, titleInputField.text, feedbackInputField.text);
        return feedbackData;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
    protected override void SetupPopupData(BaseSetupData data)
    {
        if (string.IsNullOrEmpty(userId))
        {
            userIdTxt.text = GetUserId();
        }
        userIdTxt.text = userId;
    }
}