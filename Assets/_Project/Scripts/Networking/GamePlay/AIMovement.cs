

using UnityEngine;
using UnityEngine.AI;

public class AIMovement : TGTHMonoBehaviour
{
    // Components
    private HeroLoadData heroLoadData;
    private FindTarget findTarget;
    private NavMeshAgent agent;
    // Properties
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 5f;
    // Target
    private Transform m_Target;
    public Transform Target { get { return m_Target; } }
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        if (heroLoadData != null)
        {
            heroLoadData.OnHeroDataLoaded += LoadHeroData;
        }
    }
    protected override void Start()
    {
        base.Start();
        agent.autoBraking = false;
        agent.angularSpeed = turnSpeed;
    }
    private void LoadHeroData(HeroData data)
    {
        agent.stoppingDistance = data.attackRange;
        agent.speed = data.moveSpeed;
    }
    private void Update()
    {
        FindTargetNearest();
        CheckArrived();
        RotateToMoveDirection();
    }
    [ContextMenu("Set Target to Player")]
    public void SetDefaultTarget()
    {
        if (agent != null && m_Target != null)
        {
            agent.SetDestination(m_Target.position);
        }
    }
    public void SetTarget(Transform newTarget)
    {
        m_Target = newTarget;
        if (agent != null && m_Target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(m_Target.position);
        }
    }
    public void FindTargetNearest()
    {
        if (findTarget != null && m_Target == null)
        {
            m_Target = findTarget.target;
            return;
        }
    }
    public void CheckArrived()
    {
        if (agent != null && agent.isOnNavMesh == true)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        OnArrived();
                    }
                }
            }
        }
    }
    void RotateToMoveDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(m_Target.position - transform.position);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 15f
            );
        }
    }
    void OnArrived()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }
    public bool IsMoving()
    {
        if (agent != null)
            return !agent.isStopped;
        else
            return false;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        agent = GetComponent<NavMeshAgent>();
        heroLoadData = GetComponent<HeroLoadData>();
        findTarget = GetComponent<FindTarget>();
    }

}