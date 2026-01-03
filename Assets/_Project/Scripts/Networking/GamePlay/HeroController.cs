using UnityEngine;


public class HeroController : TGTHNetworkBehaviour
{
    private IStateMachine m_playerSM;
    public AIMovement m_aiMovement;
    [HideInInspector] public IMoveable moveable;
    [HideInInspector] public Animator anim;

    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        m_playerSM = new HeroStateMachine(this);
        m_playerSM.Init<IdleState_Hero>();

    }

    override protected void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (!IsOwner) return;
        m_playerSM.Update();
    }

    override protected void LoadComponent()
    {
        base.LoadComponent();
        anim = GetComponentInChildren<Animator>();
        moveable = GetComponent<IMoveable>();
        m_aiMovement = GetComponent<AIMovement>();
    }
}