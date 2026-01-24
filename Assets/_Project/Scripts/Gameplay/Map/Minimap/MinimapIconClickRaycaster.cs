using Unity.VisualScripting;
using UnityEngine;

public class MinimapIconClickRaycaster : TGTHMonoBehaviour
{
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private LayerMask iconLayer; // MinimapIcon layer
    [SerializeField] private InputManager input;
    [Header("Focus")]
    [SerializeField] private float focusLerpSpeed = 12f;
    [SerializeField] private float focusStopDistance = 0.15f; // mét
    private bool _isFocusing;
    private Vector3 _focusTarget;

    public bool IsFocusing => _isFocusing;
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
        Vector2 screenPos = input.GetPointerPosition();

        Camera uiCam = null;
        if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCam = rootCanvas.worldCamera;

        if (!RectTransformUtility.RectangleContainsScreenPoint(minimapRect, screenPos, uiCam))
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, screenPos, uiCam, out var local);

        Rect r = minimapRect.rect;
        float u = (local.x - r.xMin) / r.width;
        float v = (local.y - r.yMin) / r.height;

        Ray ray = minimapCamera.ViewportPointToRay(new Vector3(u, v, 0f));
        if (Physics.Raycast(ray, out var hit, 5000f, iconLayer))
        {
            var icon = hit.collider.GetComponentInParent<MinimapWorldIcon>();
            if (icon != null)
            {
                icon.OnItemInteract();
                FocusToWorldPoint(hit.point);
            }
        }
    }

    public void FocusToWorldPoint(Vector3 worldPos)
    {
        if (minimapCamera == null) return;

        // giữ Y của camera, chỉ pan XZ
        _focusTarget = new Vector3(worldPos.x, minimapCamera.transform.position.y, worldPos.z);
        _isFocusing = true;
    }

    public void CancelFocus()
    {
        _isFocusing = false;
    }
    private void Update()
    {
        if (_isFocusing)
            UpdateFocus();
    }
    private void UpdateFocus()
    {
        if (!_isFocusing || minimapCamera == null) return;

        Vector3 pos = minimapCamera.transform.position;

        // Lerp mượt
        pos = Vector3.Lerp(pos, _focusTarget, focusLerpSpeed * Time.deltaTime);
        minimapCamera.transform.position = pos;

        // Stop khi đủ gần
        Vector3 flatDelta = _focusTarget - pos;
        flatDelta.y = 0f;

        if (flatDelta.sqrMagnitude <= focusStopDistance * focusStopDistance)
        {
            minimapCamera.transform.position = _focusTarget;
            _isFocusing = false;
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
