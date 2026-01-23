using UnityEngine;

public class MinimapController : TGTHMonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private InputManager input;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Canvas rootCanvas;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    [Header("Pan")]
    [SerializeField] private float panSpeed = 1f;

    private bool _prevPressed;
    private bool _panCaptured; // ✅ chỉ pan khi captured

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }

    private void Update()
    {
        if (minimapCamera == null || input == null || minimapRect == null) return;

        bool pressed = input.IsPointerPressed();
        bool over = IsPointerOverMinimap();

        // Detect press down (edge)
        if (pressed && !_prevPressed)
        {
            // ✅ chỉ capture nếu bắt đầu click trong minimap
            _panCaptured = over;
        }

        // Nếu đang giữ mà ra ngoài -> huỷ capture ngay
        // if (pressed && _panCaptured && !over)
        // {
        //     _panCaptured = false;
        // }

        // Khi nhả -> reset
        if (!pressed && _prevPressed)
        {
            _panCaptured = false;
        }

        // Zoom: chỉ khi con trỏ đang ở trong minimap
        if (over)
            HandleZoom();

        // Pan: chỉ khi đang pressed + đã captured
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
            float newSize = minimapCamera.orthographicSize - scrollY * zoomSpeed;
            minimapCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    private void HandlePan()
    {
        Vector2 delta = input.GetPointerDelta();
        if (delta.sqrMagnitude < 0.01f) return;

        float worldPerPixelY = (2f * minimapCamera.orthographicSize) / Screen.height;
        float worldPerPixelX = (2f * minimapCamera.orthographicSize * minimapCamera.aspect) / Screen.width;

        Vector3 move = new Vector3(
            -delta.x * worldPerPixelX,
            0f,
            -delta.y * worldPerPixelY
        ) * panSpeed;

        minimapCamera.transform.position += move;
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (input == null) input = FindAnyObjectByType<InputManager>();
        if (minimapRect == null) minimapRect = GetComponent<RectTransform>();
        if (rootCanvas == null && minimapRect != null) rootCanvas = minimapRect.GetComponentInParent<Canvas>();
    }
}
