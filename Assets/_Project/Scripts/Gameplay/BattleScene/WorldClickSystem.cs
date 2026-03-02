using UnityEngine;

public class WorldClickSystem : TGTHMonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager input;
    private bool _wasPressed;
    public LayerMask antiPlayer;
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
    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (input == null) input = FindAnyObjectByType<InputManager>();
    }
}
