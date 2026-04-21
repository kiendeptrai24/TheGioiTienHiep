using TMPro;
using UnityEngine;

public class InGameUIScreenManager : ScreenManager
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateInterval = 0.5f;

    private int frameCount;
    private float elapsedTime;
    [SerializeField] private GameObject m_VisualStandardScreen;
    protected override void Awake()
    {
        base.Awake();
        m_Screens.Add(m_VisualStandardScreen.gameObject.name, m_VisualStandardScreen);
    }
    protected override void Start()
    {
        base.Start();
    }
    private void OnEnable()
    {
        MinimapManger.Instance.ChangeRendertextureCameraInGameUI();
    }
    void Update()
    {
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime >= updateInterval)
        {
            float fps = frameCount / elapsedTime;
            fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";

            frameCount = 0;
            elapsedTime = 0f;
        }
    }
}
