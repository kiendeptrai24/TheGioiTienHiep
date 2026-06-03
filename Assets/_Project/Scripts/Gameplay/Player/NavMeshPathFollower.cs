using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using System.Linq;
using Unity.Netcode;
[RequireComponent(typeof(Rigidbody))]
public class NavMeshPathFollower : TGTHNetworkBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public ActorController actorController;
    public InputManager inputManager;

    [Header("Move")]
    public float arriveDistance = 0.2f;

    [Header("Debug")]
    [SerializeField] private bool hasPath;
    [SerializeField] private int currentCornerIndex;

    private readonly List<Vector3> corners = new();

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        // player tự input => hủy auto move
        if (inputManager != null &&
            inputManager.GetInputDirection() != Vector2.zero &&
            hasPath)
        {
            StopMove();
            return;
        }

        if (!hasPath || corners.Count == 0)
            return;

        if (currentCornerIndex >= corners.Count)
        {
            StopMove();
            return;
        }

        Vector3 currentTarget = corners[currentCornerIndex];

        Vector3 to = currentTarget - rb.position;
        to.y = 0f;

        // tới waypoint
        if (to.magnitude <= arriveDistance)
        {
            currentCornerIndex++;

            if (currentCornerIndex >= corners.Count)
            {
                StopMove();
            }

            return;
        }

        Vector3 dir = to.normalized;

        actorController.SetAutoMove(new Vector2(dir.x, dir.z));
    }
    public void RequestSetPath(List<Vector3> newPath)
    {
        if (!IsSpawned) return;

        var dtoPath = newPath
            .Select(p => new Vector3Dto(p))
            .ToList();

        string json = JsonConvert.SerializeObject(dtoPath);
        Debug.Log("RequestSetPath: " + json);
        SetPathServerRpc(json);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetPathServerRpc(string newPath)
    {
        SetPath(newPath);
    }
    private void SetPath(string newPath)
    {
        if (!IsServer) return;

        var dtoPath = JsonConvert.DeserializeObject<List<Vector3Dto>>(newPath);

        corners.Clear();

        if (dtoPath == null) return;

        corners.AddRange(dtoPath.Select(p => p.ToVector3()));
    }
    public void RequesMove(List<Vector3> newPath)
    {
        if (!IsSpawned) return;
        var dtoPath = newPath
           .Select(p => new Vector3Dto(p))
           .ToList();

        string json = JsonConvert.SerializeObject(dtoPath);
        MoveServerRpc(json);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void MoveServerRpc(string newPath)
    {
        var dtoPath = JsonConvert.DeserializeObject<List<Vector3Dto>>(newPath);

        corners.Clear();

        if (dtoPath == null) return;

        var path = dtoPath
            .Select(p => p.ToVector3())
            .ToList();

        Move(path);
    }

    private bool Move(List<Vector3> path)
    {
        corners.Clear();

        if (path == null) return false;

        corners.AddRange(path);


        bool success = corners.Count > 0;

        if (!success)
        {
            StopMove();
            return false;
        }

        if (corners.Count > 1)
        {
            corners.RemoveAt(0);
        }

        currentCornerIndex = 0;
        hasPath = corners.Count > 0;
        return hasPath;
    }

    public void StopMove()
    {
        hasPath = false;
        currentCornerIndex = 0;

        corners.Clear();

        actorController.ClearAutoMove();
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();

        rb = GetComponent<Rigidbody>();
        actorController = GetComponent<ActorController>();

        inputManager = FindAnyObjectByType<InputManager>();
    }
}