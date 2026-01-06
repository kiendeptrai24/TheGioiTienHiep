

using UnityEngine;

public class EnemyTarget : ISkillTarget
{
    private Transform _transform;

    public EnemyTarget(Transform transform)
    {
        _transform = transform;
    }

    public Vector3 Position => _transform.position;

    public bool IsAlive => true;

    public Vector3 Forward => _transform.forward;

    public Quaternion Rotation => _transform.rotation;

    public Vector3 Center => _transform.position + Vector3.up * 1.5f;
}