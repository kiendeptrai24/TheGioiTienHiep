using UnityEngine;

public interface ISpawnArea
{
    bool Contains(Vector3 point);
    Vector3 GetRandomPoint();
    Bounds GetBounds();
}