using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : TGTHMonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public Camera MainCamera { get { return mainCamera; } }
    public CinemachineCamera cinemachine;
    [SerializeField] private float defaultCameraSize = 70f;
    [SerializeField] private Transform defaultTarget;
    protected override void Awake()
    {
        base.Awake();
        cinemachine.Lens.FieldOfView = defaultCameraSize;
    }
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
