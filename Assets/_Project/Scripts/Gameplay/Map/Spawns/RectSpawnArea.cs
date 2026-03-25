using UnityEngine;

[System.Serializable]
public class RectSpawnArea : ISpawnArea
{
    public Vector3 center;
    public Vector2 size;

    public RectSpawnArea(Vector3 center, Vector2 size)
    {
        this.center = center;
        this.size = size;
    }

    public bool Contains(Vector3 point)
    {
        Vector3 local = point - center;
        return Mathf.Abs(local.x) <= size.x * 0.5f &&
               Mathf.Abs(local.z) <= size.y * 0.5f;
    }

    public Vector3 GetRandomPoint()
    {
        float x = Random.Range(-size.x * 0.5f, size.x * 0.5f);
        float z = Random.Range(-size.y * 0.5f, size.y * 0.5f);
        return center + new Vector3(x, 0f, z);
    }

    public Bounds GetBounds()
    {
        return new Bounds(center, new Vector3(size.x, 0.1f, size.y));
    }
}