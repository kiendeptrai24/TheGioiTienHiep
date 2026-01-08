using System;
using UnityEngine;


public class HeroController : TGTHNetworkBehaviour, ISkillCaster
{
    private IStateMachine m_heroSM;
    private HeroLoadData m_heroLoadData;
    public AIMovement m_aiMovement;
    public HeroBaseSkill skillController;
    public SkillDataRuntime currentSkillData;
    private StatsData stats;
    [SerializeField] public HeroData heroData;
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
    public Vector3 Center => transform.position + Vector3.up * 1.5f;

    public ulong Id => OwnerClientId;

    public GameObject Target => gameObject;

    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        m_heroLoadData.OnHeroDataLoaded += LoadHeroData;
        m_heroSM = new HeroStateMachine(this);
        m_heroSM.Init<IdleState_Hero>();
    }

    private void LoadHeroData(HeroData data) => heroData = data;

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
        m_heroLoadData = GetComponent<HeroLoadData>();
        moveable = GetComponent<IMoveable>();
        m_aiMovement = GetComponent<AIMovement>();
        skillController = GetComponent<HeroBaseSkill>();
        stats = GetComponent<StatsData>();
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

    public StatsData GetStats() => stats;
}