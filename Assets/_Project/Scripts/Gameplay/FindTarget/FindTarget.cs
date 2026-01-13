

using UnityEngine;

public class FindTarget : TGTHMonoBehaviour, ISkillTarget 
{
    private ISkillCaster skillCaster;
    public Transform target;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float range = 10f;
    [SerializeField] private float checkInterval = 1f;
    [SerializeField] private float checkTimer = 0f;
    public Vector3 Position => target == null ? Vector3.zero : target.position;
    public Vector3 Forward => target == null ? Vector3.zero : target.forward;
    public Quaternion Rotation => target == null ? Quaternion.identity : target.rotation;
    public bool IsAlive => true;

    public Vector3 Center => GetCenter();
    private Vector3 GetCenter()
    {
        if(target == null) return Vector3.zero;
        return target.position + Vector3.up * 1.5f;
    }
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    protected override void Start()
    {
        base.Start();
        FindTargetNearest(transform.position, range);
    }
    private void Update() {
        if(target == null && Time.time >= checkInterval + checkTimer)
        {
            checkTimer = Time.time;
            Debug.Log("Finding Target...");
            FindTargetNearest(transform.position, range);
        }
    }
    public void FindTargetNearest(Vector3 fromPosition, float range)
    {
        Collider[] colliders = Physics.OverlapSphere(fromPosition, range, whatIsTarget);
        float nearestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (var collider in colliders)
        {
            if(collider.transform == transform)
                continue;
            if(collider.gameObject.GetComponent<ISkillCaster>().TeamId == skillCaster.TeamId)
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
    protected override void LoadComponent()
    {
        base.LoadComponent();
        skillCaster = GetComponent<ISkillCaster>();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}