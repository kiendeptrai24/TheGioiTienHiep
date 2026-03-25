using System.Collections.Generic;
using UnityEngine;

public class GridSpawnPattern : ISpawnPattern
{
    public List<Vector3> GeneratePoints(ISpawnArea area, SpawnSettings settings)
    {
        List<Vector3> points = new();
        Bounds bounds = area.GetBounds();

        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minZ = bounds.min.z;
        float maxZ = bounds.max.z;

        for (float x = minX; x <= maxX; x += settings.spacing)
        {
            for (float z = minZ; z <= maxZ; z += settings.spacing)
            {
                Vector3 p = new Vector3(x, bounds.center.y, z) + settings.originOffset;
                if (area.Contains(p))
                {
                    points.Add(p);
                    if (points.Count >= settings.count)
                        return points;
                }
            }
        }

        return points;
    }
}