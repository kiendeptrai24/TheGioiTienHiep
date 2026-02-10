

using UnityEngine;
using UnityEngine.AI;

public class AIChampionMovement : TGTHMonoBehaviour
{
    // Components
    private StatsData statsData;
    private FindTarget findTarget;
    private NavMeshAgent agent;
    // Properties
    [SerializeField] private float turnSpeed = 5f;
    // Target
    private Transform m_Target;
    public Transform Target { get { return m_Target; } }
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        if (statsData != null)
        {
            statsData.SetupDataPreset();
            agent.stoppingDistance = statsData.AttackSpeed;
            agent.speed = statsData.MovementSpeed;
        }
    }
    protected override void Start()
    {
        base.Start();
        agent.autoBraking = false;
        agent.angularSpeed = turnSpeed;
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
                    OnArrived();
                }
            }
        }
    }
    void RotateToMoveDirection()
    {
        if (m_Target == null) return;
        // if (agent.velocity.sqrMagnitude > 0.01f)
        // {
        Quaternion targetRot = Quaternion.LookRotation(m_Target.position - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * 15f
        );
        // }
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
        statsData = GetComponent<StatsData>();
        findTarget = GetComponent<FindTarget>();
    }

}