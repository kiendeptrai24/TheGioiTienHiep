using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountPopup : BasePopup<AccountDataPopup, BasePopupData>
{
    [SerializeField] private Image avatar;
    [SerializeField] private TextMeshProUGUI usernameTxt;
    [SerializeField] private TextMeshProUGUI userIdTxt;
    [SerializeField] private TextMeshProUGUI realmTxt;
    [SerializeField] private Button changeAccountBtn;
    [SerializeField] private Button logoutBtn;
    [SerializeField] private Button closeBtn;
    private Action onLogout;
    private Action onChangeAccount;

    override protected void Awake()
    {
        base.Awake();
        changeAccountBtn.onClick.AddListener(OnChangeAccountClicked);
        logoutBtn.onClick.AddListener(OnLogoutClicked);
        closeBtn.onClick.AddListener(OnCloseClicked);
    }

    private void OnCloseClicked()
    {
        OnCancelClicked();
    }

    private void OnLogoutClicked()
    {
        onLogout?.Invoke();
        Hide();
    }

    private void OnChangeAccountClicked()
    {
        onChangeAccount?.Invoke();
        Hide();
    }
    public void ShowPopup(AccountDataPopup data, Action<BasePopupData> onConfirm = null, Action onCancel = null, Action onLogout = null, Action onChangeAccount = null)
    {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        this.onLogout = onLogout;
        this.onChangeAccount = onChangeAccount;
        SetupPopupData(data);
        PopupManager.Instance.ShowPopup<AccountPopup>(this);
    }
    public override void Show()
    {
        base.Show();
    }
    protected override void SetupButtons()
    {
        base.SetupButtons();
    }
    public override void Hide()
    {
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

    protected override void SetupPopupData(AccountDataPopup data)
    {
        if (data == null) return;
        avatar.sprite = data.currentProfile.itemIcon;
        realmTxt.text = EnumTranslator.ToVietnamese(data.currentProfile.realmType);
        usernameTxt.text = data.username;
        userIdTxt.text = data.userId;
    }
}