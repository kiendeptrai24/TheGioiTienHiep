using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using WorldMap.UI;
public enum ActorState
{
    TopDown,
    FirstPerson,
    ThirdPerson
}
public class ActorController : TGTHNetworkBehaviour
{
    private Rigidbody rig;
    public ActorState currentState = ActorState.TopDown;
    [Header("Components")]
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float moveSpeed = 5f;

    private IRotable characterRotation;
    private InputManager inputManager;
    public IMoveable moveable;
    private bool _autoMove;
    private Vector2 _autoDir;
    Vector2 inputDirection = Vector2.zero;

    private NetworkTransform nt;
    public NetworkVariable<Vector2> Direction = new(
        Vector2.zero,
        NetworkVariableReadPermission.Owner,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<Vector2> OldDirection = new(
        Vector2.zero,
        NetworkVariableReadPermission.Owner,
        NetworkVariableWritePermission.Server
    );
    public void SetAutoMove(Vector2 dir)
    {
        _autoMove = true;
        _autoDir = dir;
    }

    public void ClearAutoMove()
    {
        if (!IsServer) return;
        _autoMove = false;
        _autoDir = Vector2.zero;
        Debug.Log("ClearAutoMove");
        StopServerRpc();
    }
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        if (currentState == ActorState.TopDown)
        {
            characterRotation = new TopDownRotation(rig);
            moveable = new TopDownMovement(rig);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Direction.OnValueChanged += HandleDirectionChanged;
    }

    private void HandleDirectionChanged(Vector2 previousValue, Vector2 newValue)
    {
        moveable.Move(newValue, moveSpeed);
    }
    public void TelePort(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        if (!IsServer) return;
        StopServerRpc();
        nt.Teleport(pos, rot, scale);
    }
    private void FixedUpdate()
    {
        if (currentState == ActorState.TopDown)
            TopDownControl();
    }

    private void TopDownControl()
    {

        if (IsOwner)
        {
            inputDirection = inputManager.GetInputDirection();
            if (inputDirection.sqrMagnitude < 0.0001f && OldDirection.Value.sqrMagnitude < 0.0001f)
                return;
            RequestMoveServerRpc(inputDirection);
        }
        else if (IsServer)
        {
            if (_autoMove == false) return;

            inputDirection = _autoDir;
            if (inputDirection.sqrMagnitude < 0.0001f && OldDirection.Value.sqrMagnitude < 0.0001f)
                return;
            Move(inputDirection);
        }

    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void RequestMoveServerRpc(Vector2 dir)
    {
        Move(dir);
    }
    private void Move(Vector2 dir)
    {
        if (!IsServer) return;
        OldDirection.Value = Direction.Value;
        Direction.Value = dir;
        if (dir.sqrMagnitude < 0.1f)
        {
            moveable.Move(Vector2.zero, 0);
            return;
        }

        Vector2 inputDirection = dir;
        moveable.Move(inputDirection, moveSpeed);
        characterRotation.Rotate(new Vector3(inputDirection.x, 0, inputDirection.y), turnSpeed);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Server)]
    public void StopServerRpc()
    {
        moveable.Move(Vector2.zero, 0);
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        inputManager = FindAnyObjectByType<InputManager>();
        rig = GetComponent<Rigidbody>();
        nt = GetComponent<NetworkTransform>();
    }
}