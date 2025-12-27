using UnityEngine;
using UnityEngine.UI;

public class ChatPanelManager : TGTHMonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private Toggle zoomToggle;
    [SerializeField] private Image arrowImg;

    [Header("Panels")]
    [SerializeField] private GameObject chatPanel;
    protected override void Awake()
    {
        base.Awake();
        zoomToggle.onValueChanged.AddListener(OnZoomToggle);
    }

    protected override void Start()
    {
        OnZoomToggle(zoomToggle.isOn);
    }

    private void OnZoomToggle(bool isOn)
    {
        chatPanel.SetActive(isOn);
        if(isOn)
            arrowImg.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        else
            arrowImg.rectTransform.rotation = Quaternion.Euler(0, 0, 180);

    }

}
