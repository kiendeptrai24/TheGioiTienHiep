

using UnityEngine;
using UnityEngine.AI;

public class AIChampionMovement : TGTHMonoBehaviour
{
    // Components
    private StatsData statsData;
    private NavMeshAgent agent;
    // Properties
    [SerializeField] private float turnSpeed = 5f;
    // Target
    private TargetFinderBase targetFinder;
    public Transform Target => targetFinder.Target;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        if (statsData != null)
        {
            agent.stoppingDistance = 0.5f;
            agent.speed = statsData.MovementSpeed;
        }
    }
    protected override void Start()
    {
        base.Start();
        agent.angularSpeed = turnSpeed;
    }
    private void Update()
    {
        CheckArrived();
        RotateToMoveDirection();
    }
    [ContextMenu("Set Target to Player")]
    public void SetDefaultTarget()
    {
        if (agent != null && Target != null)
        {
            agent.SetDestination(Target.position);
        }
    }
    public void SetDetinition(Transform newTarget)
    {
        if (newTarget == null || newTarget == Target)
            return;
        targetFinder.SetTarget(newTarget);
    }
    public void SetDetinition(Vector3 destination)
    {
        if (agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
        }
    }
    public void CheckArrived()
    {
        if (agent != null && agent.isOnNavMesh == true)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance && agent.isStopped == false)
                {
                    OnArrived();
                }
            }
        }
    }
    void RotateToMoveDirection()
    {
        if (Target == null) return;
        Quaternion targetRot = Quaternion.LookRotation(Target.position - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * 15f
        );
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
        targetFinder = GetComponent<TargetFinderBase>();
    }

}