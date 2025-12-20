
using UnityEngine;
using UnityEngine.UI;

public abstract class ActionButton : TGTHMonoBehaviour
{
    private Button m_Button;
    [SerializeField] protected ScreenManager m_ScreenManager;

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
        m_Button.onClick.AddListener(OnClick);
        if(m_ScreenManager == null)
            m_ScreenManager = GetComponentInParent<ScreenManager>(); 

    }
}