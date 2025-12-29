using System.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePopup<TData, TResult> : TGTHMonoBehaviour, IPopup
    where TData : IPopupData
{
    [Header("Base Popup UI")]
    [SerializeField] private bool closeWhenClickOkBtn = true;
    [SerializeField] private bool closeWhenClickCancelBtn = true;
    [SerializeField] protected Button okBtn;
    [Header("Animation Popup")]
    [SerializeField] protected RectTransform rect;
    [SerializeField] protected CanvasGroup group;

    protected Action<TResult> onConfirm;
    protected Action onCancel;

    public bool IsVisible => gameObject.activeInHierarchy;

    protected override void Awake()
    {
        base.Start();
        LoadComponent();
        SetupButtons();
        //Hide();
    }

    protected virtual void SetupButtons()
    {
        if (okBtn != null)
        {
            okBtn.onClick.AddListener(OnOkClicked);
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowPopup(TData data, Action<TResult> onConfirm = null, Action onCancel = null)
    {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;

        SetupPopupData(data);
        PopupManager.Instance.ShowPopup<BasePopup<TData, TResult>>(this);
    }

    protected abstract void SetupPopupData(TData data);

    protected virtual void OnOkClicked()
    {
        var result = GetResult();
        if (ValidateResult(result))
        {
            onConfirm?.Invoke(result);
            if (!closeWhenClickOkBtn) return;
            PopupManager.Instance.HidePopup(this);
        }
    }

    protected virtual void OnCancelClicked()
    {
        onCancel?.Invoke();
        if (!closeWhenClickCancelBtn) return;
        PopupManager.Instance.HidePopup(this);
    }

    protected abstract TResult GetResult();
    protected virtual bool ValidateResult(TResult result) => true;

    protected char ValidateChar(string validCharacters, char addedChar)
    {
        if (string.IsNullOrEmpty(validCharacters) || validCharacters.Contains(addedChar))
        {
            return addedChar;
        }
        return '\0';
    }
    protected override void LoadComponent()
    {
        group = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }
}
