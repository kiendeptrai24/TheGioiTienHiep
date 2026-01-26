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
    [SerializeField] private MapSpawn mapSpawn;
    public MapSearchController mapSearchController;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float moveSpeed = 5f;
    private IRotable characterRotation;
    private InputManager inputManager;
    public IMoveable moveable;
    private bool _autoMove;
    private Vector2 _autoDir;
    public void SetAutoMove(Vector2 dir)
    {
        _autoMove = true;
        _autoDir = dir;
    }

    public void ClearAutoMove()
    {
        _autoMove = false;
        _autoDir = Vector2.zero;
        Stop();
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
        mapSearchController.actorController = this;

    }
    protected override void Start()
    {
        base.Start();
        mapSpawn = FindAnyObjectByType<MapSpawn>();
        mapSpawn.player = transform;
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

        // deadzone
        if (inputDirection.sqrMagnitude < 0.0001f)
        {
            moveable.Move(transform, Vector2.zero, 0);
            return;
        }

        characterRotation.Rotate(transform, new Vector3(inputDirection.x, 0, inputDirection.y), turnSpeed);
        moveable.Move(transform, inputDirection, moveSpeed);
    }
    public void Move(Vector2 dir)
    {
        if (!IsOwner) return;
        Vector2 inputDirection = dir;
        moveable.Move(transform, inputDirection, moveSpeed);
        characterRotation.Rotate(transform, new Vector3(inputDirection.x, 0, inputDirection.y), turnSpeed);
    }
    public void RequestTeleport(Vector3 pos, Quaternion rot)
    {
        Debug.Log("RequestTeleport");
        RequestTeleportServerRpc(pos, rot);

    }
    [ServerRpc]
    public void RequestTeleportServerRpc(Vector3 pos, Quaternion rot)
    {
        Debug.Log("RequestTeleportServerRpc");
        // SERVER xác nhận quyền
        TeleportInternal(pos, rot);

        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { OwnerClientId }
            }
        };

        TeleportClientRpc(pos, rot, rpcParams);
    }
    private void TeleportInternal(Vector3 pos, Quaternion rot)
    {
        Debug.Log("TeleportInternal");
        rig.position = pos;
        rig.rotation = rot;
        rig.linearVelocity = Vector3.zero;
    }

    [ClientRpc]
    private void TeleportClientRpc(
        Vector3 pos,
        Quaternion rot,
        ClientRpcParams rpcParams = default)
    {
        Debug.Log("TeleportClientRpc");
        rig.position = pos;
        rig.rotation = rot;
        rig.linearVelocity = Vector3.zero;
    }
    public void Stop()
    {
        moveable.Move(transform, Vector2.zero, 0);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        inputManager = FindAnyObjectByType<InputManager>();
        rig = GetComponent<Rigidbody>();
        mapSearchController = FindAnyObjectByType<MapSearchController>();
    }
}