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

    [Header("Move")]
    [Tooltip("Tốc độ di chuyển (world units / giây).")]
    [SerializeField] private float moveSpeed = 10f;

    [Tooltip("Khoảng cách tới đích thì dừng.")]
    [SerializeField] private float stopDistance = 0.05f;

    private bool _enabled;

    private bool _moving;
    private Vector3 _destination;

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
                TryClickIcon();
            };
        }

    }
    private void OnEnable() => _enabled = true;

    private void OnDisable()
    {
        _enabled = false;
        _moving = false; // ✅ không giữ lại trạng thái
    }

    private void Update()
    {
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

    public void TryClickIcon()
    {
        if (minimapCamera == null || minimapRect == null || input == null || target == null) return;

        Vector2 screenPos = input.GetPointerPosition();

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

        // viewport -> raycast world
        Ray ray = minimapCamera.ViewportPointToRay(new Vector3(u, v, 0f));
        if (Physics.Raycast(ray, out var hit, 10000f, iconLayer))
        {
            var icon = hit.collider.GetComponentInParent<MinimapWorldIcon>();
            if (icon != null)
            {
                icon.OnItemInteract();
                // ✅ set điểm đến 1 lần, không theo dõi nữa
                _destination = icon.transform.position;
                _moving = true;
            }
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
