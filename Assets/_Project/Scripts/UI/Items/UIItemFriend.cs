using System;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
public class UIItemFriend : TGTHMonoBehaviour, IPointerClickHandler
{

    public TextMeshProUGUI nameTxt;
    private Button copyNameBtn;
    #region Events
    public event Action<UIItemFriend> OnItemClicked;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        copyNameBtn.onClick.AddListener(CopyName);
    }
    private void CopyName()
    {
        if (string.IsNullOrEmpty(nameTxt.text))
            return;

        GUIUtility.systemCopyBuffer = nameTxt.text;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemClicked?.Invoke(this);
    }
    public void SetName(string name)
    {
        nameTxt.text = name;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        nameTxt = GetComponentInChildren<TextMeshProUGUI>();
        copyNameBtn = GetComponentInChildren<Button>();
    }
}
