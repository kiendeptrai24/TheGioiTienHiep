using UnityEngine;

public class PlayerCameraControl : TGTHNetworkBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        cameraManager.SetTarget(this.transform);
    }
    override protected void LoadComponent()
    {
        base.LoadComponent();
        cameraManager = FindAnyObjectByType<CameraManager>();
    }
}
