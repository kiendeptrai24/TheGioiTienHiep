using System;
using Unity.Netcode;
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
    public NetworkVariable<Vector2> Direction = new(
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
        _autoMove = false;
        _autoDir = Vector2.zero;
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
    protected override void Start()
    {
        base.Start();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Direction.OnValueChanged += HandleDirectionChanged;
    }

    private void HandleDirectionChanged(Vector2 previousValue, Vector2 newValue)
    {
        moveable.Move(transform, newValue, moveSpeed);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (currentState == ActorState.TopDown)
            TopDownControl();
    }
    
    private void TopDownControl()
    {
        Vector2 inputDirection = _autoMove ? _autoDir : inputManager.GetInputDirection();

        MoveServerRpc(inputDirection);
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void MoveServerRpc(Vector2 dir)
    {
        if (!IsServer) return;

        Direction.Value = dir;
        if (dir.sqrMagnitude < 0.0001f)
        {
            moveable.Move(transform, Vector2.zero, 0);
            return;
        }

        Vector2 inputDirection = dir;
        moveable.Move(transform, inputDirection, moveSpeed);
        characterRotation.Rotate(transform, new Vector3(inputDirection.x, 0, inputDirection.y), turnSpeed);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void StopServerRpc()
    {
        moveable.Move(transform, Vector2.zero, 0);
    }
    
    protected override void LoadComponent()
    {
        base.LoadComponent();
        inputManager = FindAnyObjectByType<InputManager>();
        rig = GetComponent<Rigidbody>();
    }
}