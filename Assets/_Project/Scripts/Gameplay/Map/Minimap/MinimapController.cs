using System;
using Unity.Netcode;
using UnityEngine;

public class MinimapController : TGTHMonoBehaviour
{
    [SerializeField] private bool canInteract = true;
    [Header("Refs")]
    [SerializeField] private MinimapManger minimapManager;
    [SerializeField] private BoxCollider targetCollider;
    [SerializeField] private Transform target;
    [SerializeField] private Transform followPlayer;
    [SerializeField] private InputManager input;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Canvas rootCanvas;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float defaultZoom = 50f;
    [SerializeField] private float maxZoom = 100f;

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
        MinimapManger.Instance.Register(this);
        if (followPlayer == null && PlayerNetManager.Instance != null && PlayerNetManager.Instance.IsPlayerExist)
        {
            SetFollowPlayer(PlayerNetManager.Instance.GetPlayer().transform);
        }
        PlayerNetManager.Instance.OnPlayerExiststed += (player) =>
        {
            SetFollowPlayer(player.transform);
        };

    }
    private void OnEnable()
    {
        if (minimapManager == null || minimapManager.cinemachineCamera == null || canInteract == false) return;
        minimapManager.cinemachineCamera.Lens.OrthographicSize = defaultZoom;
    }
    private void OnDisable()
    {
        if (minimapManager == null || minimapManager.cinemachineCamera == null || canInteract == false) return;
        minimapManager.cinemachineCamera.Lens.OrthographicSize = defaultZoom;
    }
    private void OnDestroy()
    {
        if (MinimapManger.Instance != null)
            MinimapManger.Instance.Unregister(this);
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
        if (followPlayer != null && isFollowPlayer)
        {
            target.position = ClampToBoxCollider(followPlayer.position);
        }
        if (!canInteract) return;

        if (minimapManager == null || minimapManager.minimapCamera == null || minimapManager.cinemachineCamera == null
        || input == null || minimapRect == null || target == null) return;

        bool pressed = input.IsUIPointerPressed();
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

        if (pressed && _panCaptured && input.IsZoom() == false)
            HandlePan();

        _prevPressed = pressed;
    }

    private bool IsPointerOverMinimap()
    {
        Vector2 screenPos = input.GetUIPointerPosition();

        Camera uiCam = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCam = rootCanvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(minimapRect, screenPos, uiCam);
    }

    private void HandleZoom()
    {
        float scrollY = input.GetInputScrollWheel();
        if (Mathf.Abs(scrollY) > 0.001f)
        {
            var vcam = minimapManager.cinemachineCamera;
            float curSize = vcam.Lens.OrthographicSize;

            float newSize = curSize - scrollY * zoomSpeed;
            vcam.Lens.OrthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);

            // Re-clamp target ngay sau khi zoom để không lệch ra ngoài bounds
            if (target != null)
                target.position = ClampToBoxCollider(target.position);
        }
    }

    private void HandlePan()
    {
        Vector2 delta = input.GetUIPointerDelta();
        if (delta.sqrMagnitude < 0.01f) return;

        float worldPerPixelY = (2f * minimapManager.minimapCamera.orthographicSize) / Screen.height;
        float worldPerPixelX = (2f * minimapManager.minimapCamera.orthographicSize * minimapManager.minimapCamera.aspect) / Screen.width;

        Vector3 move = new Vector3(
            -delta.x * worldPerPixelX,
            0f,
            -delta.y * worldPerPixelY
        ) * panSpeed;

        Vector3 desired = target.position + move;

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

        // Thu hẹp vùng hợp lệ theo kích thước viewport để view không tràn ra ngoài bounds khi zoom
        if (minimapManager != null && minimapManager.cinemachineCamera != null)
        {
            float orthoH = minimapManager.cinemachineCamera.Lens.OrthographicSize;
            float camAspect = (minimapManager.minimapCamera != null) ? minimapManager.minimapCamera.aspect : 1f;
            float orthoW = orthoH * camAspect;

            min.x += orthoW; max.x -= orthoW;
            min.z += orthoH; max.z -= orthoH;
        }

        // Đảm bảo min <= max (trường hợp zoom quá lớn so với bounds)
        min.x = Mathf.Min(min.x, max.x);
        min.z = Mathf.Min(min.z, max.z);

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.z = Mathf.Clamp(pos.z, min.z, max.z);
        // giữ Y như cũ
        return pos;
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (input == null) input = InputManager.Instance;
        if (minimapRect == null) minimapRect = GetComponent<RectTransform>();
        if (rootCanvas == null && minimapRect != null) rootCanvas = minimapRect.GetComponentInParent<Canvas>();
        if (minimapManager == null) minimapManager = MinimapManger.Instance;

        // nếu bạn quên gán targetCollider, có thể tự tìm theo tag/name tuỳ bạn
        // if (targetCollider == null) targetCollider = FindAnyObjectByType<BoxCollider>();
    }
}
