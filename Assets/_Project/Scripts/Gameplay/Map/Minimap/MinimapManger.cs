using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class MinimapManger : Singleton<MinimapManger>
{
    public Camera minimapCamera;
    public CinemachineCamera cinemachineCamera;
    public Transform minimapTarget;
    private PlayerNetManager playerNetManager;
    [SerializeField] private Transform player;
    [SerializeField] private List<MinimapController> controllers = new();
    [SerializeField] private List<RenderTexture> renderTextureCameras = new();
    private RenderTexture curRendertexture;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        playerNetManager = PlayerNetManager.Instance;
        playerNetManager.OnPlayerExiststed += OnPlayerExists;
        if (renderTextureCameras.Count < 2) return;
        curRendertexture = renderTextureCameras[0];
    }
    public void Register(MinimapController c)
    {
        controllers.Add(c);
    }

    public void Unregister(MinimapController c)
        => controllers.Remove(c);

    private void OnPlayerExists(NetworkObject @object)
    {
        SetPlayer(@object.transform);
    }

    protected override void Start()
    {
        base.Start();
    }
    public void ChangeRendertextureCameraInGameUI()
    {
        if (renderTextureCameras.Count < 2) return;
        if (curRendertexture == renderTextureCameras[0]) return;
        minimapCamera.targetTexture = renderTextureCameras[0];

    }
    public void ChangeRendertextureCameraInMap()
    {
        if (renderTextureCameras.Count < 2) return;
        if (curRendertexture == renderTextureCameras[1]) return;
        minimapCamera.targetTexture = renderTextureCameras[1];
    }
    public void SetPlayer(Transform player)
    {
        this.player = player;
        if (controllers != null)
            foreach (var controller in controllers)
            {
                controller.SetFollowPlayer(player);
            }
    }
    public Transform GetPlayer() => player;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (minimapCamera == null) minimapCamera = GetComponentInChildren<Camera>();
        if (cinemachineCamera == null) cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
    }
}
