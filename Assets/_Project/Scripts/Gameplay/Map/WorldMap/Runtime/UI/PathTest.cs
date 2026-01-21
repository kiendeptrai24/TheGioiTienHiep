using System.Collections.Generic;
using UnityEngine;
using WorldMap.Domain;

public class PathTest : TGTHMonoBehaviour
{
    public MapSpawn mapSpawn;
    public Transform A;
    public Transform B;

    private readonly List<GridCoord> path = new List<GridCoord>(512);

    [ContextMenu("Find Path A->B")]
    public void Find()
    {
        var start = mapSpawn.WorldToGrid(A.position);
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

        // Debug vẽ line
        if (ok)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 p1 = mapSpawn.GridToWorld(path[i]);
                Vector3 p2 = mapSpawn.GridToWorld(path[i + 1]);
                Debug.DrawLine(p1 + Vector3.up * 0.2f, p2 + Vector3.up * 0.2f, Color.green, 3f);
            }
        }
    }
}
