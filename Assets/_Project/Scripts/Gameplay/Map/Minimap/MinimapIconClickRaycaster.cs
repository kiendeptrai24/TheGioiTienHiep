using UnityEngine;

public class MinimapIconClickRaycaster : TGTHMonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private LayerMask iconLayer; // icon phải có Collider + layer này
    [SerializeField] private InputManager input;

    private bool _enabled;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();

        // Subscribe input once
        input.inputHandler.UI.PointerPress.performed += _ =>
        {
            if (!_enabled) return;
            TryClickIcon();
        };
    }

    private void OnEnable()  => _enabled = true;
    private void OnDisable() => _enabled = false;

    public void TryClickIcon()
    {
        if (minimapCamera == null || minimapRect == null || input == null) return;

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
                icon.OnItemInteract();
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
