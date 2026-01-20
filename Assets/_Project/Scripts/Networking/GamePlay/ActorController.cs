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
    [SerializeField] private MapSpawn mapSpawn;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float moveSpeed = 5f;
    private IRotable characterRotation;
    private InputManager inputManager;
    public IMoveable moveable;
    protected override void Awake()
    {
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
        mapSpawn = FindAnyObjectByType<MapSpawn>();
        mapSpawn.Owner = transform;
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (currentState == ActorState.TopDown)
            TopDownControl();
    }

    private void TopDownControl()
    {
        Vector2 inputDirection = inputManager.GetInputDirection();
        characterRotation.Rotate(transform, new Vector3(inputDirection.x, 0, inputDirection.y), turnSpeed);
        moveable.Move(transform, inputDirection, moveSpeed);
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        inputManager = FindAnyObjectByType<InputManager>();
        rig = GetComponent<Rigidbody>();
    }
}