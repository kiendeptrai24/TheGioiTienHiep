
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ActionButton : TGTHMonoBehaviour
{
    [SerializeField] public ScreenManager screenManager;
    private Button m_Button;
    public Action m_OnClick;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    public abstract void OnClick();
    protected override void LoadComponent()
    {
        base.LoadComponent();
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(() => 
        {
            m_OnClick?.Invoke(); 
            OnClick();
        });
        if (screenManager == null)
            screenManager = GetComponentInParent<ScreenManager>();

    }
}