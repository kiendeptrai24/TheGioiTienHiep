using UnityEngine;

public class MinimapController : TGTHMonoBehaviour
{
    [SerializeField] private bool canInteract = true;
    [Header("Refs")]
    [SerializeField] private MinimapManger minimapManager;
    [SerializeField] private BoxCollider targetCollider; // ✅ vùng giới hạn (World bounds)
    [SerializeField] private Transform target;
    [SerializeField] private Transform followPlayer;
    [SerializeField] private InputManager input;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Canvas rootCanvas;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    [Header("Pan")]
    [SerializeField] private float panSpeed = 1f;

    [Header("Clamp")]
    [Tooltip("Nếu true: clamp theo collider bounds nhưng trừ half-size của target để không lọt ra ngoài.")]
    [SerializeField] private bool considerTargetSize = true;

    private bool _prevPressed;
    private bool _panCaptured;
    [SerializeField] private bool isFollowPlayer = false;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }

    protected override void Start()
    {
        base.Start();
    }
    public void SetFollowPlayer(Transform player)
    {
        this.followPlayer = player;
    }
    private void Update()
    {

        if (isFollowPlayer && followPlayer != null)
        {
            target.position = ClampToBoxCollider(followPlayer.position);
        }
        if (!canInteract) return;

        if (minimapManager == null || minimapManager.minimapCamera == null || minimapManager.cinemachineCamera == null
        || input == null || minimapRect == null || target == null) return;

        bool pressed = input.IsPointerPressed();
        bool over = IsPointerOverMinimap();

        if (pressed && !_prevPressed)
        {
            _panCaptured = over; // chỉ capture nếu click bắt đầu trong minimap
        }

        if (!pressed && _prevPressed)
        {
            _panCaptured = false;
        }

        if (over)
            HandleZoom();

        if (pressed && _panCaptured)
            HandlePan();

        _prevPressed = pressed;
    }

    private bool IsPointerOverMinimap()
    {
        Vector2 screenPos = input.GetPointerPosition();

        Camera uiCam = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCam = rootCanvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(minimapRect, screenPos, uiCam);
    }

    private void HandleZoom()
    {
        float scrollY = input.GetInputScrollWheel().y;
        if (Mathf.Abs(scrollY) > 0.001f)
        {
            var vcam = minimapManager.cinemachineCamera; // CinemachineVirtualCamera
            float curSize = vcam.Lens.OrthographicSize;

            float newSize = curSize - scrollY * zoomSpeed;
            vcam.Lens.OrthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    private void HandlePan()
    {
        Vector2 delta = input.GetPointerDelta();
        if (delta.sqrMagnitude < 0.01f) return;

        float worldPerPixelY = (2f * minimapManager.minimapCamera.orthographicSize) / Screen.height;
        float worldPerPixelX = (2f * minimapManager.minimapCamera.orthographicSize * minimapManager.minimapCamera.aspect) / Screen.width;

        Vector3 move = new Vector3(
            -delta.x * worldPerPixelX,
            0f,
            -delta.y * worldPerPixelY
        ) * panSpeed;

        Vector3 desired = target.position + move;

        // ✅ Clamp target trong BoxCollider

        desired = ClampToBoxCollider(desired);

        target.position = desired;
    }

    private Vector3 ClampToBoxCollider(Vector3 pos)
    {
        if (targetCollider == null) return pos;

        Bounds b = targetCollider.bounds; // world bounds
        Vector3 min = b.min;
        Vector3 max = b.max;

        // Nếu muốn đảm bảo "target không bị lọt ra ngoài" (tính theo kích thước target)
        if (considerTargetSize)
        {
            // Lấy bounds của target nếu có collider/renderer (ưu tiên collider)
            if (target.TryGetComponent<Collider>(out var col))
            {
                Vector3 ext = col.bounds.extents;
                min.x += ext.x; max.x -= ext.x;
                min.z += ext.z; max.z -= ext.z;
            }
            else if (target.TryGetComponent<Renderer>(out var rd))
            {
                Vector3 ext = rd.bounds.extents;
                min.x += ext.x; max.x -= ext.x;
                min.z += ext.z; max.z -= ext.z;
            }
        }

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.z = Mathf.Clamp(pos.z, min.z, max.z);
        // giữ Y như cũ (hoặc bạn muốn cố định thì chỉnh ở đây)
        return pos;
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (input == null) input = FindAnyObjectByType<InputManager>();
        if (minimapRect == null) minimapRect = GetComponent<RectTransform>();
        if (rootCanvas == null && minimapRect != null) rootCanvas = minimapRect.GetComponentInParent<Canvas>();
        if (minimapManager == null) minimapManager = FindAnyObjectByType<MinimapManger>();

        // nếu bạn quên gán targetCollider, có thể tự tìm theo tag/name tuỳ bạn
        // if (targetCollider == null) targetCollider = FindAnyObjectByType<BoxCollider>();
    }
}
