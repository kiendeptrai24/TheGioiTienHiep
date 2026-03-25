using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnPattern : ISpawnPattern
{
    public List<Vector3> GeneratePoints(ISpawnArea area, SpawnSettings settings)
    {
        List<Vector3> points = new();

        for (int i = 0; i < settings.count; i++)
        {
            bool found = false;

            for (int attempt = 0; attempt < settings.maxAttemptsPerPoint; attempt++)
            {
                Vector3 candidate = area.GetRandomPoint() + settings.originOffset;

                if (IsFarEnough(candidate, points, settings.spacing))
                {
                    points.Add(candidate);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogWarning($"Không tìm được vị trí hợp lệ cho point thứ {i}");
            }
        }

        return points;
    }

    private bool IsFarEnough(Vector3 point, List<Vector3> points, float minDistance)
    {
        float sqr = minDistance * minDistance;
        foreach (var p in points)
        {
            Vector3 d = p - point;
            d.y = 0f;
            if (d.sqrMagnitude < sqr)
                return false;
        }
        return true;
    }
}