using System.Collections.Generic;
using UnityEngine;
using WorldMap.Domain;

public class PathFinding : Singleton<PathFinding>
{
    public NavMeshPathFollower follower;
    public MapSpawn mapSpawn;
    public Transform B;
    private PathVisualizer pathVisualizer;


    public class FindPathResult
    {
        public ItemData itemData;
        public List<Vector3> path;
        public Vector3 start;
        public Vector3 goal;
        public bool ok;
        public int distance;
    }
    private readonly List<GridCoord> path = new List<GridCoord>(512);
    List<Vector3> corners = new List<Vector3>();
    protected override void Start()
    {
        base.Start();
        pathVisualizer = PathVisualizer.Instance;
    }
    [ContextMenu("Find Path A->B")]
    public void Find()
    {
        if (mapSpawn == null || follower == null || B == null)
        {
            Debug.Log("Missing references");
            return;
        }
        var start = follower.transform.position;
        var goal = B.position;

        if (NavMeshPathUtility.TryGetCorners(start, goal, out List<Vector3> corners))
        {
            if (follower != null && corners != null)
            {
                this.corners = corners;
            }
            pathVisualizer.Draw(corners);
        }
    }
    public FindPathResult FindPathWithPossition(Vector3 pos)
    {
        if (mapSpawn == null || follower == null)
        {
            return null;
        }
        var start = follower.transform.position;
        var goal = pos;
        FindPathResult result = new FindPathResult();
        result.ok = false;
        if (NavMeshPathUtility.TryGetCorners(start, goal, out List<Vector3> corners))
        {
            result.ok = true;
            result.path = corners;
            result.start = start;
            result.goal = goal;
            result.distance = path.Count;
            return result;
            
        }

        return result;
    }
    public void StartFollowPath()
    {
        if (follower != null)
        {
            follower.SetPath(corners);
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
