using UnityEngine;

public class MinimapIconClickRaycaster : TGTHMonoBehaviour
{
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private LayerMask iconLayer; // MinimapIcon layer
    [SerializeField] private InputManager input;
    private bool enable = false;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        input.inputHandler.UI.PointerPress.performed += (context) =>
        {
            if (!enable) return;
            TryClickIcon();
        };

    }
    private void OnEnable()
    {
        enable = true;
    }
    private void OnDisable()
    {
        enable = false;
    }
    public void TryClickIcon()
    {
        Vector2 screenPos = input.GetPointerPosition(); // hoặc inputManager.GetPointerScreenPosition()

        // kiểm tra có đang click trong minimap UI không
        Camera uiCam = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCam = rootCanvas.worldCamera;

        if (!RectTransformUtility.RectangleContainsScreenPoint(minimapRect, screenPos, uiCam))
            return;

        // screenPos -> localPos trong minimapRect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, screenPos, uiCam, out var local);

        // local -> normalized (0..1)
        Rect r = minimapRect.rect;
        float u = (local.x - r.xMin) / r.width;
        float v = (local.y - r.yMin) / r.height;

        // normalized -> ray từ minimapCamera
        Ray ray = minimapCamera.ViewportPointToRay(new Vector3(u, v, 0f));
        if (Physics.Raycast(ray, out var hit, 5000f, iconLayer))
        {
            var icon = hit.collider.GetComponentInParent<MinimapWorldIcon>();
            if (icon != null)
                icon.ShowInfo();
        }
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (minimapRect == null) minimapRect = GetComponent<RectTransform>();
        if (rootCanvas == null && minimapRect != null) rootCanvas = minimapRect.GetComponentInParent<Canvas>();
        input = FindAnyObjectByType<InputManager>();
    }
}
