using System;
using Unity.Netcode;
using UnityEngine;


public class HeroController : TGTHNetworkBehaviour, ISkillCaster
{
    protected StatsData stats;
    public NetworkObject attackPrefab;
    protected IStateMachine m_heroSM;
    protected HeroLoadData m_heroLoadData;
    public AIMovement m_aiMovement;
    public HeroBaseSkill skillController;
    public SkillDataRuntime currentSkillData;
    protected HealthController healthController;
    public FindTarget target;
    [SerializeField] public HeroData heroData;
    [HideInInspector] public IMoveable moveable;
    [HideInInspector] public Animator anim;
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

    public ulong Id => OwnerClientId;

    override protected void Awake()
    {
        base.Awake();
        LoadComponent();
        m_heroLoadData.OnHeroDataLoaded += LoadHeroData;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        healthController.OnDead += OnDeadServerRpc;
    }
    protected void LoadHeroData(HeroData data) => heroData = data;

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
    [ServerRpc]
    protected void OnDeadServerRpc()
    {
        NetworkObject.Despawn();
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
        healthController = GetComponent<HealthController>();
        target = GetComponent<FindTarget>();
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