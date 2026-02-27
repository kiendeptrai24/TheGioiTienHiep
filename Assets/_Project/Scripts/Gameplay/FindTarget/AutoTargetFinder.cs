using UnityEngine;

public class AutoTargetFinder : TargetFinderBase
{
    private ISkillCaster skillCaster;
    public Transform target;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float range = 10f;
    [SerializeField] private float checkInterval = 1f;
    private float checkTimer = 0f;

    // Override abstract properties from TargetFinderBase
    public override Vector3 Position => target == null ? Vector3.zero : target.position;
    public override Vector3 Forward => target == null ? Vector3.zero : target.forward;
    public override Quaternion Rotation => target == null ? Quaternion.identity : target.rotation;
    public override bool IsAlive => target != null;

    public override Vector3 Center => target == null ? Vector3.zero : target.position + Vector3.up * 1.5f;

    public override Transform Target => target;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }

    private void Update()
    {
        if (target == null && Time.time >= checkInterval + checkTimer)
        {
            checkTimer = Time.time;
            FindTargetNearest(transform.position, range);
        }
    }

    // Automatically finds and sets the nearest target
    private void FindTargetNearest(Vector3 fromPosition, float range)
    {
        Collider[] colliders = Physics.OverlapSphere(fromPosition, range, whatIsTarget);
        float nearestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (var collider in colliders)
        {
            if (collider.transform == transform)
                continue;
            if (!collider.TryGetComponent<ISkillCaster>(out var otherCaster))
                continue;

            if (otherCaster.TeamId == skillCaster.TeamId)
                continue;

            float distance = Vector3.Distance(fromPosition, collider.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = collider.transform;
            }
        }

        if (nearestTarget != null)
        {
            SetTarget(nearestTarget);
        }
    }

    // Implements the SetTarget method from the base class
    public override void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    protected override void LoadComponent()
    {
        skillCaster = GetComponent<ISkillCaster>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public override void SetTarget(Vector3 destination)
    {
        throw new System.NotImplementedException();
    }
}