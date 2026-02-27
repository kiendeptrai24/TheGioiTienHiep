using UnityEngine;

public class ManualTargetFinder : TargetFinderBase
{
    public Transform target;

    // Override abstract properties from TargetFinderBase
    public override Vector3 Position => target == null ? Vector3.zero : target.position;
    public override Vector3 Forward => target == null ? Vector3.zero : target.forward;
    public override Quaternion Rotation => target == null ? Quaternion.identity : target.rotation;
    public override bool IsAlive => target != null;

    public override Vector3 Center => target == null ? Vector3.zero : target.position + Vector3.up * 1.5f;

    public override Transform Target => target;

    // Implements the SetTarget method from the base class
    public override void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public override void SetTarget(Vector3 destination)
    {
        
    }
}