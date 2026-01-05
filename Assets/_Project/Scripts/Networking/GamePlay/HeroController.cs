using UnityEngine;


public class HeroController : TGTHNetworkBehaviour , ISkillCaster
{
    private IStateMachine m_heroSM;
    public AIMovement m_aiMovement;
    public HeroBaseSkill skillController;
    public SkillDataRuntime currentSkillData;
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

    public Vector3 Forward => transform.forward;

    public Quaternion Rotation => transform.rotation;

    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        m_heroSM = new HeroStateMachine(this);
        m_heroSM.Init<IdleState_Hero>();

    }

    override protected void Start()
    {
        base.Start();
    }
    public IStateMachine GetStateMachine()
    {
        return m_heroSM;
    }

    private void Update()
    {
        if (!IsOwner) return;
        m_heroSM.Update();
    }

    override protected void LoadComponent()
    {
        base.LoadComponent();
        anim = GetComponentInChildren<Animator>();
        moveable = GetComponent<IMoveable>();
        m_aiMovement = GetComponent<AIMovement>();
        skillController = GetComponent<HeroBaseSkill>();
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