using UnityEngine;

public class JoystickChangePosition : TGTHMonoBehaviour
{
    [SerializeField] private RectTransform targetRect;
    private RectTransform rectTransform;

    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private float extraOffset = 200f;

    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float radius = targetRect.rect.width * targetRect.localScale.x;
        Vector2 pos = direction.normalized * (radius + extraOffset);
        rectTransform.anchoredPosition = pos;
    }
}