

using UnityEngine;
using UnityEngine.AI;

public class AIMovement : TGTHMonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public Transform target;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 5f;
    private NavMeshAgent agent;
    [ContextMenu("Set Target to Player")]
    public void SetDefaultTarget()
    {
        if (agent != null && target != null)
        {
            agent.SetDestination(target.position);
        }
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (agent != null && target != null)
        {
            agent.SetDestination(target.position);
        }
    }
    protected override void Start() {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.stoppingDistance = stoppingDistance;
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        SetTarget(target);
    }
    public void UpdateDetination() {
        if(target != null && agent != null && agent.isOnNavMesh == true)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }
    private void Update() {
        if(playerController != null && playerController.IsOwner && playerController.moveable.IsMoving())
        {
            UpdateDetination();
        }
        if(agent != null && agent.isOnNavMesh == true)
        {
            if(!agent.pathPending)
            {
                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    if(!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        OnArrived();
                    }
                }
            }
        }
        if(playerController != null && playerController.IsOwner && IsMoving())
        {
            RotateToMoveDirection();
        }
    }
    void RotateToMoveDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 dir = agent.velocity.normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 10f
            );
        }
    }
    void OnArrived()
    {
        agent.isStopped = true;
    }
    public bool IsMoving()
    {
        if(agent != null)
            return !agent.isStopped;
        else
            return false;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        agent = GetComponent<NavMeshAgent>();
        playerController = FindAnyObjectByType<PlayerController>();
        target = playerController.transform;
    }

}