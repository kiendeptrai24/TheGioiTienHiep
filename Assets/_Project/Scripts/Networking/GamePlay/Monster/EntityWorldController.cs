
using UnityEngine;

public class EntityWorldController : TGTHNetworkBehaviour
{
    public Animator anim;
    protected IStateMachine m_entitySM;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        if (!IsOwner || m_entitySM == null) return;
        m_entitySM.Update();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        anim = GetComponentInChildren<Animator>();
    }
}