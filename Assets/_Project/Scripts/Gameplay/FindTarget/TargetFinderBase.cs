
using UnityEngine;

public abstract class TargetFinderBase : TGTHMonoBehaviour, ISkillTarget
{
    // Abstract properties, to be implemented in the derived class
    public abstract Vector3 Position { get; }
    public abstract Vector3 Forward { get; }
    public abstract Quaternion Rotation { get; }
    public abstract bool IsAlive { get; }
    public abstract Vector3 Center { get; }
    public abstract Transform Target { get; }


    // Abstract method for setting the target, to be implemented in the derived classes
    public abstract void SetTarget(Transform newTarget);
    public abstract void SetTarget(Vector3 destination);
}