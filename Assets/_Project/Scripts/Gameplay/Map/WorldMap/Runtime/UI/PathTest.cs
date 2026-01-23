using System.Collections.Generic;
using UnityEngine;
using WorldMap.Domain;

public class PathTest : TGTHMonoBehaviour
{
    public PathFollowerRB follower;
    public MapSpawn mapSpawn;
    public Transform B;

    private readonly List<GridCoord> path = new List<GridCoord>(512);

    [ContextMenu("Find Path A->B")]
    public void Find()
    {
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
