using FeatureToggles;
using UnityEngine;

public class WorldClickSystem : TGTHMonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager input;
    private bool _wasPressed;
    public LayerMask antiPlayer;
    protected override void Awake()
    {
        base.Awake();
        input = FindAnyObjectByType<InputManager>();
        mainCamera = Camera.main;
    }
    private void Update()
    {
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

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~antiPlayer))
        {
            if (hit.collider.TryGetComponent<IWorldClickable>(out var clickable))
            {
                clickable.OnClicked();
            }
        }
    }
}
