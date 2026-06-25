using UnityEngine;

public class MinimapCameraLimiter : MonoBehaviour
{
    public Camera minimapCamera;
    public float updateInterval = 0.02f;

    private float timer;

    void Awake()
    {
        if (minimapCamera == null)
            minimapCamera = GetComponentInChildren<Camera>();

        minimapCamera.enabled = false;
    }

    void Update()
    {
        if (Time.time >= updateInterval + timer)
        {
            timer = Time.time;
            minimapCamera.Render();
        }
    }
}