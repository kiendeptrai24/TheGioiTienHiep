using UnityEngine;
using UnityEngine.UI;

public abstract class ActionToggle : TGTHMonoBehaviour
{
    [SerializeField] protected ScreenManager m_ScreenManager;
    private Toggle m_Toggle;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start() {
        OnClick(m_Toggle.isOn);
    }
    public abstract void OnClick(bool isOn);
    protected override void LoadComponent()
    {
        base.LoadComponent();
        m_Toggle = GetComponent<Toggle>();
        m_Toggle.onValueChanged.AddListener(OnClick);
        if(m_ScreenManager == null)
            m_ScreenManager = GetComponentInParent<ScreenManager>(); 

    }
}
