using UnityEngine;

public class UIFollow : TGTHMonoBehaviour
{
    [SerializeField] private Transform worldTarget; // quái hoặc UIAnchor
    [SerializeField] private RectTransform uiRect;
    [SerializeField] private Vector3 offset;

    private Camera mainCam;

    protected override void Awake()
    {
        LoadComponent();
    }

    protected override void Start()
    {
        uiRect.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (worldTarget == null) return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(
            worldTarget.position + offset
        );

        // Nếu quái sau camera → ẩn UI
        if (screenPos.z < 0)
        {
            uiRect.gameObject.SetActive(false);
            return;
        }

        uiRect.gameObject.SetActive(true);
        uiRect.position = screenPos;
    }
    public void SetTarget(Transform target)
    {
        worldTarget = target;
    }
    protected override void LoadComponent()
    {
        mainCam = Camera.main;
    }
}
