using ExitGames.Client.Photon.StructWrapping;
using FeatureToggles;
using Unity.Netcode;
using UnityEngine;

public class WorldClickSystem : TGTHMonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager input;
    private bool _wasPressed;
    public LayerMask antiPlayer;
    public LayerMask whatIsGround;
    public PathFollowerRB pathFollowerRB;
    private bool canClick = false;
    protected override void Awake()
    {
        base.Awake();
        input = FindAnyObjectByType<InputManager>();
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
    }
    private void OnPlayerExists(NetworkObject playerNet)
    {
        this.pathFollowerRB = playerNet.GetComponent<PathFollowerRB>();
        canClick = true;
    }
    protected override void Start()
    {
        base.Start();
        mainCamera = Camera.main;
    }
    private void Update()
    {
        if (!canClick) return;
        bool pressed = input.IsPointerPressed();

        if (pressed && !_wasPressed)
        {
            HandleClick();
        }

        _wasPressed = pressed;
    }
    private void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(input.GetPointerPosition());
        if (Physics.Raycast(ray, out RaycastHit hit1, 1000f, whatIsGround))
        {
            if (hit1.point != null)
            {
                var findPathResult = PathFinding.Instance.FindPathWithPossition(hit1.point);
                if (findPathResult == null) return;
                var newPath = findPathResult.path;
                if (pathFollowerRB != null)
                    pathFollowerRB.SetPath(newPath);
            }
        }
        if (Physics.Raycast(ray, out RaycastHit hit2, 1000f, antiPlayer))
        {
            if (hit2.collider.TryGetComponent<IWorldClickable>(out var clickable))
            {
                var playerNet = hit2.collider.GetComponent<NetworkObject>();
                if (playerNet != null)
                {
                    if (playerNet.IsOwner)
                        return;
                }
                clickable.OnClicked();
            }
        }
    }
}
