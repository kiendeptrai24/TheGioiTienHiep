

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

    public Vector3 Forward => throw new System.NotImplementedException();

    public Quaternion Rotation => throw new System.NotImplementedException();
}