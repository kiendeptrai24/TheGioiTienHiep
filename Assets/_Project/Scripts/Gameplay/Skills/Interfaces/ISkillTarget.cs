using UnityEngine;

public interface ISkillTarget
{
    Vector3 Center { get; }
    Vector3 Position { get; }
    Vector3 Forward { get; }
    Quaternion Rotation { get; }
    bool IsAlive { get; }

}