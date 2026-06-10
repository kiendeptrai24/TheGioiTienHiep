using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PathFinding : Singleton<PathFinding>
{
    private NavMeshPathFollower follower;
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
    public List<Vector3> corners = new List<Vector3>();
    protected override void Start()
    {
        base.Start();
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExiststed;

        pathVisualizer = PathVisualizer.Instance;
    }

    private void OnPlayerExiststed(NetworkObject playerNet)
    {
        follower = playerNet.GetComponent<NavMeshPathFollower>();
    }

    [ContextMenu("Find Path A->B")]
    public void Find()
    {
        if (follower == null || B == null)
        {
            Debug.Log("Missing references");
            return;
        }
        this.corners.Clear();
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
        if (follower == null)
        {
            return null;
        }
        var start = follower.transform.position;
        var goal = pos;
        this.corners.Clear();
        FindPathResult result = new FindPathResult();
        result.ok = false;
        if (NavMeshPathUtility.TryGetNearestReachablePoint(goal, 3, out Vector3 pointNearest))
            if (NavMeshPathUtility.TryGetCorners(start, pointNearest, out List<Vector3> corners))
            {
                float totalDistance = 0f;

                for (int i = 0; i < corners.Count - 1; i++)
                {
                    totalDistance += Vector3.Distance(
                        corners[i],
                        corners[i + 1]
                    );
                }
                this.corners = corners;
                result.ok = true;
                result.path = corners;
                result.start = start;
                result.goal = goal;
                result.distance = Mathf.RoundToInt(totalDistance);
                return result;

            }

        return result;
    }
    public void StartFollowPath()
    {
        if (follower != null)
        {
            follower.RequesMove(corners);
        }
    }
}
