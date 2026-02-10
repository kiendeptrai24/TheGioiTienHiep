using UnityEngine;


public class ChampionController : TGTHMonoBehaviour, ISkillCaster
{
    protected StatsData stats;
    public GameObject attackPrefab;
    protected IStateMachine m_championSM;
    public AIChampionMovement m_aiMovement;
    public HeroBaseSkill skillController;
    public SkillDataRuntime currentSkillData;
    protected HealthController healthController;
    [HideInInspector] public IMoveable moveable;
    [HideInInspector] public Animator anim;
    public Transform Target => m_target;

    [SerializeField] protected float _mana = 100f;
    [SerializeField] protected float _stamina = 100f;
    [SerializeField] protected int _teamId = 0;

    public float Mana => _mana;
    public float Stamina => _stamina;
    public int TeamId => _teamId;
    public Vector3 Position => transform.position;
    public bool IsAlive => true;
    public Vector3 Forward => transform.forward;
    public Quaternion Rotation => transform.rotation;
    public Vector3 Center => transform.position + Vector3.up * 1.5f;

    public ulong Id => 1;

    public Transform m_target;
    public void SetTeamId(int teamId)
    {
        _teamId = teamId;
    }
    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        healthController.OnDead += OnDead;
    }

    override protected void Start()
    {
        base.Start();
    }
    public IStateMachine GetStateMachine()
    {
        return m_championSM;
    }

    private void Update()
    {
        m_championSM.Update();
    }
    protected void OnDead()
    {
        Destroy(gameObject);
    }

    override protected void LoadComponent()
    {
        base.LoadComponent();
        anim = GetComponentInChildren<Animator>();
        moveable = GetComponent<IMoveable>();
        m_aiMovement = GetComponent<AIChampionMovement>();
        skillController = GetComponent<HeroBaseSkill>();
        stats = GetComponent<StatsData>();
        healthController = GetComponent<HealthController>();
    }
    public StatsData GetStats() => stats;

    public void ConsumeMana(float amount)
    {
        _mana -= amount;
    }

    public void ConsumeStamina(float amount)
    {
        _stamina -= amount;
    }

    public bool HasState(string stateId)
    {
        return false;
    }
}