using UnityEngine;

public abstract class ActionNavigation : TGTHMonoBehaviour 
{
    [SerializeField] protected ScreenManager m_ScreenManager;
    [SerializeField] protected string m_ScreenName;
    private EffectManager m_EffectManager;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    public virtual void OnClick()
    {
        if(m_EffectManager == null)
            m_EffectManager = EffectManager.Instance;
        m_EffectManager.PlayOneShot("button-click");
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        if(m_ScreenManager == null)
            m_ScreenManager = GetComponentInParent<ScreenManager>(); 

    }
}

