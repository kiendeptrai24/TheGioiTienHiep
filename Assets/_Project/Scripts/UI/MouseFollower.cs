using UnityEngine;

public class MouseFollower : TGTHMonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private InputManager inputManager;
    private UIInventoryItem item;

    protected override void Awake()
    {
        LoadComponent();
    }

    protected override void LoadComponent()
    {
        canvas = transform.root.GetComponent<Canvas>();
        item = GetComponentInChildren<UIInventoryItem>(true);
    }
    public void SetData(Sprite sprite, int quantity)
    {
        item.SetData(sprite, quantity);
    }
    void Update()
    {
        if(inputManager  == null) return;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            inputManager.GetUIInputDirection(),
            canvas.worldCamera,
            out position
                );
        transform.position = canvas.transform.TransformPoint(position);
    }
    
    public void Toggle(bool val)
    {
        gameObject.SetActive(val);
    }
}