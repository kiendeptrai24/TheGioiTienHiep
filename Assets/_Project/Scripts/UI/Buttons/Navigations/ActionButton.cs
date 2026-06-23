
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ActionButton : TGTHMonoBehaviour
{
    [SerializeField] public ScreenManager screenManager;
    private Button m_Button;
    public Action m_OnClick;
    private EffectManager m_EffectManager;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    public virtual void OnClick()
    {
        if (m_EffectManager == null)
            m_EffectManager = EffectManager.Instance;
        m_EffectManager.PlayOneShot("button-click");
    }
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