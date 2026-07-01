using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
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
    private bool lockMove = false;
    private NetworkTransform nt;
    private Vector2 _lastAppliedDirection = Vector2.zero;
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
        SetAutoMoveInternal(dir);

        if (UsesServerAuthority || HasMovementAuthority)
            return;

        if (IsServer)
            SetAutoMoveClientRpc(dir, RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
    }

    public void ClearAutoMove()
    {
        ClearAutoMoveInternal();

        if (HasMovementAuthority)
        {
            StopMovement();
            return;
        }

        if (UsesServerAuthority && IsOwner)
        {
            RequestStopServerRpc();
            return;
        }

        if (!UsesServerAuthority && IsServer)
            ClearAutoMoveClientRpc(RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
    }
    public void TelePort(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        if (HasMovementAuthority)
        {
            StartCoroutine(TeleportRoutine(pos, rot, scale));
            return;
        }

        if (!UsesServerAuthority && IsServer)
        {
            ApplyMirrorTeleportState(pos, rot, scale);
            TeleportClientRpc(pos, rot, scale, RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
        }
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
        if (!UsesServerAuthority || HasMovementAuthority)
            return;

        _lastAppliedDirection = newValue;
        moveable.Move(newValue, moveSpeed);
    }
    public bool UsesServerAuthority => nt == null || nt.IsServerAuthoritative();
    public bool HasMovementAuthority
    {
        get
        {
            if (!IsSpawned)
                return false;

            return UsesServerAuthority ? IsServer : IsOwner;
        }
    }
    private void SetAutoMoveInternal(Vector2 dir)
    {
        _autoMove = true;
        _autoDir = dir;
    }

    private void ClearAutoMoveInternal()
    {
        _autoMove = false;
        _autoDir = Vector2.zero;
    }

    private void ApplyMirrorTeleportState(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        lockMove = true;
        rig.linearVelocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(pos, rot);
        transform.localScale = scale;
        lockMove = false;
    }

    private IEnumerator TeleportRoutine(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        lockMove = true;

        // dừng Rigidbody trước
        rig.linearVelocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        yield return new WaitForFixedUpdate();

        nt.Teleport(pos, rot, scale);

        lockMove = false;
    }
    private void FixedUpdate()
    {
        if (currentState == ActorState.TopDown)
            TopDownControl();
    }
    private void TopDownControl()
    {
        if (UsesServerAuthority)
        {
            if (HasMovementAuthority && _autoMove)
            {
                inputDirection = _autoDir;
                if (inputDirection.sqrMagnitude < 0.0001f && _lastAppliedDirection.sqrMagnitude < 0.0001f)
                    return;
                MoveAuthority(inputDirection);
                return;
            }

            if (IsOwner)
            {
                if (inputManager == null) return;
                inputDirection = inputManager.GetInputDirection();
                if (inputDirection.sqrMagnitude < 0.0001f && _lastAppliedDirection.sqrMagnitude < 0.0001f)
                    return;
                RequestMoveServerRpc(inputDirection);
                return;
            }

            return;
        }
        else
        {
            if (!IsOwner) return;

            if (inputManager == null) return;

            var manualDirection = inputManager.GetInputDirection();
            inputDirection = _autoMove && manualDirection.sqrMagnitude < 0.0001f
                ? _autoDir
                : manualDirection;

            if (inputDirection.sqrMagnitude < 0.0001f && _lastAppliedDirection.sqrMagnitude < 0.0001f)
                return;

            MoveAuthority(inputDirection);
        }
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void RequestMoveServerRpc(Vector2 dir)
    {
        if (!UsesServerAuthority)
            return;

        MoveAuthority(dir);
    }
    private void MoveAuthority(Vector2 dir)
    {
        if (!HasMovementAuthority) return;
        if (lockMove) return;

        if (UsesServerAuthority)
            OldDirection.Value = Direction.Value;

        if (dir.sqrMagnitude < 0.1f)
        {
            if (UsesServerAuthority && _autoMove == false)
                Direction.Value = dir;
            moveable.Move(Vector2.zero, 0);
            _lastAppliedDirection = Vector2.zero;
            return;
        }

        if (UsesServerAuthority)
        {
            Direction.Value = dir;
        }

        _lastAppliedDirection = dir;

        Vector2 currentInputDirection = dir;
        moveable.Move(currentInputDirection, moveSpeed);
        characterRotation.Rotate(new Vector3(currentInputDirection.x, 0, currentInputDirection.y), turnSpeed);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void RequestStopServerRpc()
    {
        if (!UsesServerAuthority)
            return;

        StopMovement();
    }

    private void StopMovement()
    {
        moveable.Stop();
        _lastAppliedDirection = Vector2.zero;

        if (UsesServerAuthority)
        {
            HandleDirectionChanged(Vector2.zero, Vector2.zero);
            Direction.Value = Vector2.zero;
            OldDirection.Value = Vector2.zero;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetAutoMoveClientRpc(Vector2 dir, RpcParams rpcParams = default)
    {
        if (!IsOwner)
            return;

        SetAutoMoveInternal(dir);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ClearAutoMoveClientRpc(RpcParams rpcParams = default)
    {
        if (!IsOwner)
            return;

        ClearAutoMoveInternal();
        StopMovement();
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void TeleportClientRpc(Vector3 pos, Quaternion rot, Vector3 scale, RpcParams rpcParams = default)
    {
        if (!IsOwner)
            return;

        StartCoroutine(TeleportRoutine(pos, rot, scale));
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        inputManager = FindAnyObjectByType<InputManager>();
        rig = GetComponent<Rigidbody>();
        nt = GetComponent<NetworkTransform>();
    }
}
