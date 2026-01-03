using UnityEngine;

public interface ISkillTarget
{
    Vector3 Position { get; }
    bool IsAlive { get; }
}