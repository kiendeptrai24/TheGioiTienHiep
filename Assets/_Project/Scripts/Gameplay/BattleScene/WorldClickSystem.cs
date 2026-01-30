using UnityEngine;

public class WorldClickSystem : TGTHMonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager input;

    private void Update()
    {
        if (!input.IsPointerPressed())
            return;

        Ray ray = mainCamera.ScreenPointToRay(input.GetPointerPosition());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
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
