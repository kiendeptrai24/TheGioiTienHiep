using UnityEngine;

public class SafeZoneManager : Singleton<SafeZoneManager>
{
    [SerializeField] private Vector3 center;
    [SerializeField] private Vector3 size;

    public bool IsInside(Vector3 position)
    {
        Vector3 min = center - size * 0.5f;
        Vector3 max = center + size * 0.5f;

        return position.x >= min.x && position.x <= max.x &&
               position.z >= min.z && position.z <= max.z;
    }
    public bool OutSide(Vector3 position)
    {
        Vector3 min = center - size * 0.5f;
        Vector3 max = center + size * 0.5f;

        return position.x < min.x || position.x > max.x ||
               position.z < min.z || position.z > max.z;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}