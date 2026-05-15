using System.Collections.Generic;
using UnityEngine;
using WorldMap.Domain;

[RequireComponent(typeof(Rigidbody))]
public class PathFollowerRB : TGTHNetworkBehaviour
{
    public MapSpawn mapSpawn;
    public PathFinding pathTest;
    public ActorController actorController;
    public Rigidbody rb;
    public InputManager inputManager;
    public float arriveDistance = 0.15f;
    public int lookAheadSteps = 4;     // càng lớn càng “cong”
    public float cornerSlowDown = 0.8f; // optional
    private readonly List<GridCoord> gridPath = new();
    private int index = -1;
    [SerializeField] private bool hasPath;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (inputManager.GetInputDirection() != Vector2.zero && hasPath)
        {
            Stop();
            return;
        }
        if (!hasPath || mapSpawn == null || gridPath.Count == 0) return;

        // nếu hết path
        if (index >= gridPath.Count)
        {
            Stop();
            return;
        }

        Vector3 target = mapSpawn.GridToWorld(gridPath[index]);
        Vector3 pos = rb.position;

        // top-down: bỏ y để không bị ảnh hưởng độ cao (nếu bạn muốn bám heightY thì giữ y theo target.y)
        Vector3 to = target - pos;
        to.y = 0f;

        // tới waypoint
        if (to.magnitude <= arriveDistance)
        {
            index++;
            if (index >= gridPath.Count)
            {
                Stop();
                return;
            }
            return;
        }
        Vector3 dir = to.normalized;
        actorController.SetAutoMove(new Vector2(dir.x, dir.z));
    }

    public void SetPath(List<GridCoord> newPath)
    {
        gridPath.Clear();
        gridPath.AddRange(newPath);
        if (gridPath.Count > 2)
        {
            gridPath.RemoveAt(0);
        }
        index = 0;
        hasPath = gridPath.Count > 0;
    }

    public void Stop()
    {
        hasPath = false;
        index = -1;
        actorController.ClearAutoMove();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        rb = GetComponent<Rigidbody>();
        actorController = GetComponent<ActorController>();
        mapSpawn = FindAnyObjectByType<MapSpawn>();
        inputManager = FindAnyObjectByType<InputManager>();
        pathTest = PathFinding.Instance;
    }
}
