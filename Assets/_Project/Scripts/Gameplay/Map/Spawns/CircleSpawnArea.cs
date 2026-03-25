using UnityEngine;

[System.Serializable]
public class CircleSpawnArea : ISpawnArea
{
    public Vector3 center;
    public float radius;

    public CircleSpawnArea(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public bool Contains(Vector3 point)
    {
        Vector3 flat = point - center;
        flat.y = 0;
        return flat.sqrMagnitude <= radius * radius;
    }

    public Vector3 GetRandomPoint()
    {
        Vector2 p = Random.insideUnitCircle * radius;
        return center + new Vector3(p.x, 0f, p.y);
    }

    public Bounds GetBounds()
    {
        return new Bounds(center, new Vector3(radius * 2f, 0.1f, radius * 2f));
    }
}