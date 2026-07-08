using System;
using UnityEngine;

public class MinimapIconClickRaycaster : TGTHMonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private Transform target;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private LayerMask iconLayer; // icon phải có Collider + layer này
    [SerializeField] private InputManager input;
    [SerializeField] private LayerMask groundLayer; // world phải có Collider + layer này
    public Action<Vector3> OnDestinationChanged;
    [Header("Move")]
    [Tooltip("Tốc độ di chuyển (world units / giây).")]
    [SerializeField] private float moveSpeed = 10f;

    [Tooltip("Khoảng cách tới đích thì dừng.")]
    [SerializeField] private float stopDistance = 0.05f;

    private bool _enabled;
    private bool _moving;
    private Vector3 _destination;
    public Vector3 destinationXZ = new();

    // Pan/Zoom detection
    private Vector2 _pressStartPos;
    private bool _isPressed;
    private bool _hasZoomed;
    private const float DRAG_THRESHOLD = 10f;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void Start()
    {
        base.Start();
        if (input != null && input.inputHandler != null)
        {
            input.inputHandler.UI.PointerPress.performed += _ =>
            {
                if (!_enabled) return;
                _pressStartPos = input.GetUIPointerPosition();
                _isPressed = true;
                _hasZoomed = false;
            };

            input.inputHandler.UI.PointerPress.canceled += _ =>
            {
                if (!_enabled || !_isPressed) return;
                _isPressed = false;

                // Nếu có zoom thì bỏ qua
                if (_hasZoomed) return;

                // Nếu có lướt (pan) thì bỏ qua
                Vector2 endPos = input.GetUIPointerPosition();
                if (Vector2.Distance(_pressStartPos, endPos) > DRAG_THRESHOLD) return;

                if (TryClickIcon()) return;
                TryClickWorld();
            };
        }
    }
    private void OnEnable()
    {
        _enabled = true;
    }
    private void OnDisable()
    {
        _enabled = false;
        _moving = false;
        _isPressed = false;
        _hasZoomed = false;
    }

    private void Update()
    {
        // Track zoom trong lúc đang giữ chuột
        if (_isPressed && input != null && input.IsZoom())
        {
            _hasZoomed = true;
        }

        if (!_enabled || !_moving || target == null) return;

        Vector3 current = target.position;
        Vector3 dest = _destination;

        // chỉ di chuyển theo XZ, giữ nguyên Y
        dest.y = current.y;

        Vector3 delta = dest - current;
        delta.y = 0f;

        if (delta.sqrMagnitude <= stopDistance * stopDistance)
        {
            target.position = new Vector3(dest.x, current.y, dest.z);
            _moving = false; // ✅ tới nơi là dừng, không giữ lại
            return;
        }

        target.position = Vector3.MoveTowards(
            current,
            new Vector3(dest.x, current.y, dest.z),
            moveSpeed * Time.deltaTime
        );
    }

    public bool TryClickIcon()
    {
        if (minimapCamera == null || minimapRect == null || input == null || target == null) return false;

        Vector2 screenPos = input.GetUIPointerPosition();

        Camera uiCam = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCam = rootCanvas.worldCamera;

        // chỉ xử lý khi con trỏ nằm trong minimap UI
        if (!RectTransformUtility.RectangleContainsScreenPoint(minimapRect, screenPos, uiCam))
            return false;

        // screen -> local in rect
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, screenPos, uiCam, out var local))
            return false;

        // local -> normalized (0..1)
        Rect r = minimapRect.rect;
        float u = (local.x - r.xMin) / r.width;
        float v = (local.y - r.yMin) / r.height;
        if (u < 0f || u > 1f || v < 0f || v > 1f) return false;

        // viewport -> raycast world
        Ray ray = minimapCamera.ViewportPointToRay(new Vector3(u, v, 0f));
        if (Physics.Raycast(ray, out var hit, 10000f, iconLayer))
        {
            var icon = hit.collider.GetComponentInParent<MinimapWorldIcon>();
            if (icon != null)
            {
                //_moving = true;
                _destination = icon.transform.position;
                destinationXZ.x = icon.transform.position.x;
                destinationXZ.z = icon.transform.position.z;
                OnDestinationChanged?.Invoke(destinationXZ);
                return true;
            }
        }
        return false;
    }
    private void TryClickWorld()
    {
        if (minimapCamera == null || minimapRect == null || input == null || target == null) return;

        Vector2 screenPos = input.GetUIPointerPosition();

        Camera uiCam = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCam = rootCanvas.worldCamera;

        // chỉ xử lý khi con trỏ nằm trong minimap UI
        if (!RectTransformUtility.RectangleContainsScreenPoint(minimapRect, screenPos, uiCam))
            return;

        // screen -> local in rect
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, screenPos, uiCam, out var local))
            return;

        // local -> normalized (0..1)
        Rect r = minimapRect.rect;
        float u = (local.x - r.xMin) / r.width;
        float v = (local.y - r.yMin) / r.height;
        if (u < 0f || u > 1f || v < 0f || v > 1f) return;

        Ray ray = minimapCamera.ViewportPointToRay(new Vector3(u, v, 0f));
        if (Physics.Raycast(ray, out var ground, 100f, groundLayer))
        {
            destinationXZ.x = ground.point.x;
            destinationXZ.z = ground.point.z;
            OnDestinationChanged?.Invoke(destinationXZ);
        }
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (input == null) input = FindAnyObjectByType<InputManager>();
        if (minimapRect == null) minimapRect = GetComponent<RectTransform>();
        if (rootCanvas == null && minimapRect != null) rootCanvas = minimapRect.GetComponentInParent<Canvas>();
    }
}
