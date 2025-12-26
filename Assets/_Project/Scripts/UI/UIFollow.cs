using UnityEngine;

public class UIFollow : MonoBehaviour
{
    [SerializeField] private Transform worldTarget; // quái hoặc UIAnchor
    [SerializeField] private RectTransform uiRect;
    [SerializeField] private Vector3 offset;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void LateUpdate()
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
}
