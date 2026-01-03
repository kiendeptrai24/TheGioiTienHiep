using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : TGTHMonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public Camera MainCamera { get { return mainCamera; } }
    public CinemachineCamera cinemachine;
    [SerializeField] private float defaultCameraSize = 5f;
    [SerializeField] private Transform defaultTarget;
    [ContextMenu("Set Target")]
    public void SetDefaultTarget()
    {
        SetTarget(defaultTarget);
    }
    public void SetTarget(Transform target)
    {
        if (cinemachine != null)
        {
            cinemachine.Follow = target;
            cinemachine.LookAt = target;
        }
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (cinemachine == null)
            cinemachine = GetComponentInChildren<CinemachineCamera>();
    }
}
