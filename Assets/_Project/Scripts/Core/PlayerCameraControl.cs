using UnityEngine;

public class PlayerCameraControl : TGTHMonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        cameraManager.SetTarget(this.transform);
    }
    override protected void LoadComponent()
    {
        base.LoadComponent();
        cameraManager = FindAnyObjectByType<CameraManager>();
    }
}
