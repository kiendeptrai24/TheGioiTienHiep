using UnityEngine;


public class HeroController : TGTHNetworkBehaviour , ISkillCaster
{
    private IStateMachine m_playerSM;
    public AIMovement m_aiMovement;
    [HideInInspector] public IMoveable moveable;
    [HideInInspector] public Animator anim;
    [SerializeField] private float _mana = 100f;
    [SerializeField] private float _stamina = 100f;
    [SerializeField] private int _teamId = 0;
    public float Mana => _mana;
    public float Stamina => _stamina;
    public int TeamId => _teamId;
    public Vector3 Position => transform.position;

    public bool IsAlive => true;

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

    public void ConsumeMana(float amount)
    {
        
    }

    public void ConsumeStamina(float amount)
    {
        
    }

    public bool HasState(string stateId)
    {
        return false;
    }
}