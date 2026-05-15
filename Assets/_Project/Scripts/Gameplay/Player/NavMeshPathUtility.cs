using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshPathUtility
{
    /// <summary>
    /// Check có path hay không.
    /// </summary>
    public static bool CanReach(Vector3 start, Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();

        bool success = NavMesh.CalculatePath(
            start,
            target,
            NavMesh.AllAreas,
            path
        );

        if (!success)
            return false;

        return path.status == NavMeshPathStatus.PathComplete;
    }

    /// <summary>
    /// Lấy corners path.
    /// </summary>
    public static List<Vector3> GetCorners(Vector3 start, Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();

        bool success = NavMesh.CalculatePath(
            start,
            target,
            NavMesh.AllAreas,
            path
        );

        if (!success)
            return new List<Vector3>();

        if (path.status != NavMeshPathStatus.PathComplete)
            return new List<Vector3>();

        return new List<Vector3>(path.corners);
    }

    /// <summary>
    /// Check + lấy corners luôn.
    /// </summary>
    public static bool TryGetCorners(
        Vector3 start,
        Vector3 target,
        out List<Vector3> corners)
    {
        corners = new List<Vector3>();

        NavMeshPath path = new NavMeshPath();

        bool success = NavMesh.CalculatePath(
            start,
            target,
            NavMesh.AllAreas,
            path
        );

        if (!success)
            return false;

        if (path.status != NavMeshPathStatus.PathComplete)
            return false;

        corners.AddRange(path.corners);

        return corners.Count > 0;
    }
}