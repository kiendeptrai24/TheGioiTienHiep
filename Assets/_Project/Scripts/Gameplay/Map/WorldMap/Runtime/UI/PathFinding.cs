using System.Collections.Generic;
using UnityEngine;
using WorldMap.Domain;

public class PathFinding : Singleton<PathFinding>
{
    public PathFollowerRB follower;
    public MapSpawn mapSpawn;
    public Transform B;
    public class FindPathResult
    {
        public ItemData itemData;
        public List<GridCoord> path;
        public GridCoord start;
        public GridCoord goal;
        public bool ok;
        public int distance;
    }
    private readonly List<GridCoord> path = new List<GridCoord>(512);

    [ContextMenu("Find Path A->B")]
    public void Find()
    {
        if (mapSpawn == null || follower == null || B == null)
        {
            Debug.Log("Missing references");
            return;
        }
        var start = mapSpawn.WorldToGrid(follower.transform.position);
        var goal = mapSpawn.WorldToGrid(B.position);
        Debug.Log("Start: " + start.ToString());
        Debug.Log("Goal: " + goal.ToString());
        if (start.x < 0 || start.z < 0 || start.x > 1000 || start.z > 1000)
        {
            Debug.Log("Invalid start");
            return;
        }
        if (goal.x < 0 || goal.z < 0 || goal.x > 1000 || goal.z > 1000)
        {
            Debug.Log("Invalid goal");
            return;
        }


        bool ok = GridAStarPathfinder.TryFindPath(mapSpawn.mapDataPreset, start, goal, path);
        Debug.Log("Path ok: " + ok + " len=" + path.Count);

        if (ok)
        {
            if (follower != null)
            {
                follower.mapSpawn = mapSpawn;
                SimplifyInPlace(path);
                follower.SetPath(path);
            }
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 p1 = mapSpawn.GridToWorld(path[i]);
                Vector3 p2 = mapSpawn.GridToWorld(path[i + 1]);
                Debug.DrawLine(p1 + Vector3.up * 0.2f, p2 + Vector3.up * 0.2f, Color.green, 100f);
            }
        }
    }
    public FindPathResult FindPathWithPossition(Vector3 pos)
    {
        if (mapSpawn == null || follower == null)
        {
            return null;
        }
        var start = mapSpawn.WorldToGrid(follower.transform.position);
        var goal = mapSpawn.WorldToGrid(pos);

        if (start.x < 0 || start.z < 0 || start.x > 1000 || start.z > 1000)
        {
            Debug.Log("Invalid start");
            return null;
        }
        if (goal.x < 0 || goal.z < 0 || goal.x > 1000 || goal.z > 1000)
        {
            Debug.Log("Invalid goal");
            return null;
        }
        bool ok = GridAStarPathfinder.TryFindPath(mapSpawn.mapDataPreset, start, goal, path);
        FindPathResult result = new FindPathResult();
        result.ok = ok;
        result.path = path;
        result.start = start;
        result.goal = goal;
        result.distance = path.Count;
        return result;

    }
    public void StartFollowPath()
    {
        if (follower != null)
        {
            follower.mapSpawn = mapSpawn;
            SimplifyInPlace(path);
            follower.SetPath(path);
        }
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 p1 = mapSpawn.GridToWorld(path[i]);
            Vector3 p2 = mapSpawn.GridToWorld(path[i + 1]);
            Debug.DrawLine(p1 + Vector3.up * 0.2f, p2 + Vector3.up * 0.2f, Color.green, 100f);
        }
    }
    public static void SimplifyInPlace(List<GridCoord> path)
    {
        if (path == null || path.Count < 3) return;

        int write = 1;
        GridCoord prev = path[0];
        GridCoord cur = path[1];

        int lastDx = cur.x - prev.x;
        int lastDz = cur.z - prev.z;

        for (int i = 2; i < path.Count; i++)
        {
            GridCoord next = path[i];
            int dx = next.x - cur.x;
            int dz = next.z - cur.z;

            // nếu đổi hướng -> giữ cur
            if (dx != lastDx || dz != lastDz)
            {
                path[write++] = cur;
                lastDx = dx; lastDz = dz;
            }

            prev = cur;
            cur = next;
        }

        path[write++] = path[^1];
        path.RemoveRange(write, path.Count - write);
    }

}
