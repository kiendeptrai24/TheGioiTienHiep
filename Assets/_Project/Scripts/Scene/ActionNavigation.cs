using UnityEngine;

public abstract class ActionNavigation : TGTHMonoBehaviour 
{
    [SerializeField] protected ScreenManager m_ScreenManager;
    [SerializeField] protected string m_ScreenName;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    public abstract void OnClick();
    protected override void LoadComponent()
    {
        base.LoadComponent();
        if(m_ScreenManager == null)
            m_ScreenManager = GetComponentInParent<ScreenManager>(); 

    }
}

